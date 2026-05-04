using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Shifts;
using ShiftHub.Domain.Entities;
using ShiftHub.Domain.Enums;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.Infrastructure.Services.Shifts;

public class ShiftService : IShiftService
{
    private readonly ShiftHubDbContext _db;
    private readonly ICurrentTenantService _tenant;

    public ShiftService(ShiftHubDbContext db, ICurrentTenantService tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<Shift> CreateAsync(CreateShiftRequest request)
    {
        var startTime = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc);
        var endTime = DateTime.SpecifyKind(request.EndTime, DateTimeKind.Utc);

        if (endTime <= startTime)
            throw new InvalidOperationException("End time must be after start time.");

        var totalMinutes = (endTime - startTime).TotalMinutes;

        int breakMinutes;
        if (totalMinutes >= 360)
            breakMinutes = Math.Max(request.BreakMinutes ?? 20, 20);
        else
            breakMinutes = 0;

        var shift = new Shift
        {
            Id = Guid.NewGuid(),
            OrgId = _tenant.OrgId!.Value,
            SiteId = request.SiteId,
            PayRateId = request.PayRateId,
            StartTime = startTime,
            EndTime = endTime,
            SlotsNeeded = request.SlotsNeeded,
            BreakMinutes = breakMinutes,
            Status = ShiftStatus.Draft,
            Notes = request.Notes,
            CreatedById = _tenant.UserId!.Value,
            CreatedAt = DateTime.UtcNow
        };

        _db.Shifts.Add(shift);
        await _db.SaveChangesAsync();

        return shift;
    }

    public async Task<Shift> PublishAsync(Guid shiftId)
    {
        var shift = await _db.Shifts.FindAsync(shiftId)
            ?? throw new InvalidOperationException("Shift not found.");

        if (shift.Status != ShiftStatus.Draft)
            throw new InvalidOperationException("Only draft shifts can be published.");

        shift.Status = ShiftStatus.Reserved;
        await _db.SaveChangesAsync();

        return shift;
    }

    public async Task<Shift> OpenAsync(Guid shiftId)
    {
        var shift = await _db.Shifts.FindAsync(shiftId)
            ?? throw new InvalidOperationException("Shift not found.");

        if (shift.Status != ShiftStatus.Reserved)
            throw new InvalidOperationException("Only reserved shifts can be opened to self-serve.");

        shift.Status = ShiftStatus.Published;
        await _db.SaveChangesAsync();

        return shift;
    }

    public async Task<ShiftAssignment> AssignAsync(Guid shiftId, Guid userId)
    {
        var shift = await _db.Shifts
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.Id == shiftId)
            ?? throw new InvalidOperationException("Shift not found.");

        if (shift.Status != ShiftStatus.Reserved && shift.Status != ShiftStatus.Published)
            throw new InvalidOperationException("Shift must be reserved or published to assign workers.");

        var membership = await _db.OrgMemberships
            .FirstOrDefaultAsync(m => m.OrgId == shift.OrgId && m.UserId == userId)
            ?? throw new InvalidOperationException("User is not a member of this organisation.");

        if (membership.Status != MembershipStatus.Active)
            throw new InvalidOperationException("User is not an active member of this organisation.");

        if (membership.Role != UserRole.Worker)
            throw new InvalidOperationException("Only workers can be assigned to shifts.");

        var acceptedCount = shift.Assignments.Count(a => a.Status == AssignmentStatus.Accepted);
        if (acceptedCount >= shift.SlotsNeeded)
            throw new InvalidOperationException("This shift is already full.");

        if (shift.Assignments.Any(a => a.UserId == userId))
            throw new InvalidOperationException("This worker is already assigned to this shift.");

        var assignment = new ShiftAssignment
        {
            Id = Guid.NewGuid(),
            ShiftId = shiftId,
            UserId = userId,
            Status = AssignmentStatus.Accepted,
            AcceptedAt = DateTime.UtcNow
        };

        if (acceptedCount + 1 == shift.SlotsNeeded)
            shift.Status = ShiftStatus.Filled;

        _db.ShiftAssignments.Add(assignment);
        await _db.SaveChangesAsync();

        return assignment;
    }

    public async Task<List<Shift>> GetAvailableAsync()
    {
        return await _db.Shifts
            .Include(s => s.Site)
                .ThenInclude(s => s.Client)
            .Include(s => s.PayRate)
            .Where(s => s.Status == ShiftStatus.Published &&
                        s.Assignments.Count(a => a.Status == AssignmentStatus.Accepted) < s.SlotsNeeded)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<List<Shift>> GetAllAsync(ShiftStatus? status)
    {
        var query = _db.Shifts
            .Include(s => s.Site)
                .ThenInclude(s => s.Client)
            .Include(s => s.PayRate)
            .Include(s => s.Assignments)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(s => s.Status == status.Value);

        return await query.OrderByDescending(s => s.StartTime).ToListAsync();
    }

    public async Task<Shift> GetByIdAsync(Guid shiftId)
    {
        return await _db.Shifts
            .Include(s => s.Site)
                .ThenInclude(s => s.Client)
            .Include(s => s.PayRate)
            .Include(s => s.Assignments)
                .ThenInclude(a => a.User)
            .Include(s => s.Assignments)
                .ThenInclude(a => a.Timesheet)
            .FirstOrDefaultAsync(s => s.Id == shiftId)
            ?? throw new InvalidOperationException("Shift not found.");
    }

    public async Task<List<ShiftAssignment>> GetMyShiftsAsync()
    {
        var userId = _tenant.UserId!.Value;

        return await _db.ShiftAssignments
            .Include(a => a.Shift)
                .ThenInclude(s => s.Site)
                    .ThenInclude(s => s.Client)
            .Include(a => a.Shift)
                .ThenInclude(s => s.PayRate)
            .Include(a => a.Timesheet)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Shift.StartTime)
            .ToListAsync();
    }

    public async Task<List<Timesheet>> GetMyTimesheetsAsync()
    {
        var userId = _tenant.UserId!.Value;

        return await _db.Timesheets
            .Include(t => t.Assignment)
                .ThenInclude(a => a.Shift)
                    .ThenInclude(s => s.Site)
            .Include(t => t.Assignment)
                .ThenInclude(a => a.Shift)
                    .ThenInclude(s => s.PayRate)
            .Where(t => t.Assignment.UserId == userId)
            .OrderByDescending(t => t.ClockIn)
            .ToListAsync();
    }

    public async Task<ShiftAssignment> AcceptAsync(Guid shiftId)
    {
        var shift = await _db.Shifts
            .Include(s => s.Assignments)
            .FirstOrDefaultAsync(s => s.Id == shiftId)
            ?? throw new InvalidOperationException("Shift not found.");

        if (shift.Status != ShiftStatus.Published)
            throw new InvalidOperationException("This shift is not available.");

        var acceptedCount = shift.Assignments.Count(a => a.Status == AssignmentStatus.Accepted);
        if (acceptedCount >= shift.SlotsNeeded)
            throw new InvalidOperationException("This shift is already full.");

        var userId = _tenant.UserId!.Value;

        var alreadyAssigned = shift.Assignments.Any(a => a.UserId == userId);
        if (alreadyAssigned)
            throw new InvalidOperationException("You have already accepted this shift.");

        var assignment = new ShiftAssignment
        {
            Id = Guid.NewGuid(),
            ShiftId = shiftId,
            UserId = userId,
            Status = AssignmentStatus.Accepted,
            AcceptedAt = DateTime.UtcNow
        };

        if (acceptedCount + 1 == shift.SlotsNeeded)
            shift.Status = ShiftStatus.Filled;

        _db.ShiftAssignments.Add(assignment);
        await _db.SaveChangesAsync();

        return assignment;
    }

    public async Task ClockInAsync(Guid shiftId)
    {
        var userId = _tenant.UserId!.Value;

        var assignment = await _db.ShiftAssignments
            .FirstOrDefaultAsync(a => a.ShiftId == shiftId && a.UserId == userId && a.Status == AssignmentStatus.Accepted)
            ?? throw new InvalidOperationException("No active assignment found for this shift.");

        var alreadyClockedIn = await _db.Timesheets
            .AnyAsync(t => t.AssignmentId == assignment.Id);

        if (alreadyClockedIn)
            throw new InvalidOperationException("You have already clocked in for this shift.");

        var timesheet = new Timesheet
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignment.Id,
            ClockIn = DateTime.UtcNow
        };

        _db.Timesheets.Add(timesheet);
        await _db.SaveChangesAsync();
    }

    public async Task ClockOutAsync(Guid shiftId)
    {
        var userId = _tenant.UserId!.Value;

        var assignment = await _db.ShiftAssignments
            .FirstOrDefaultAsync(a => a.ShiftId == shiftId && a.UserId == userId)
            ?? throw new InvalidOperationException("No assignment found for this shift.");

        var timesheet = await _db.Timesheets
            .FirstOrDefaultAsync(t => t.AssignmentId == assignment.Id && t.ClockOut == null)
            ?? throw new InvalidOperationException("You have not clocked in yet.");

        timesheet.ClockOut = DateTime.UtcNow;
        timesheet.HoursWorked = (decimal)(timesheet.ClockOut.Value - timesheet.ClockIn).TotalHours;

        await _db.SaveChangesAsync();
    }
}
