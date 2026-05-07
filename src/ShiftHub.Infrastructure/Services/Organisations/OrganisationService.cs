using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Organisations;
using ShiftHub.Domain.Entities;
using ShiftHub.Domain.Enums;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.Infrastructure.Services.Organisations;

public class OrganisationService : IOrganisationService
{
    private readonly ShiftHubDbContext _db;
    private readonly ICurrentTenantService _tenant;

    public OrganisationService(ShiftHubDbContext db, ICurrentTenantService tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<Organisation> CreateAsync(CreateOrganisationRequest request)
    {
        var adminUserId = _tenant.UserId
            ?? throw new UnauthorizedAccessException("You must be logged in to create an agency.");

        var subdomainTaken = await _db.Organisations
            .AnyAsync(o => o.Subdomain == request.Subdomain.ToLower().Trim());

        if (subdomainTaken)
            throw new InvalidOperationException("This subdomain is already taken.");

        var org = new Organisation
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Subdomain = request.Subdomain.ToLower().Trim(),
            BillingEmail = request.BillingEmail,
            PayFrequency = request.PayFrequency,
            CreatedAt = DateTime.UtcNow
        };

        var membership = new OrgMembership
        {
            Id = Guid.NewGuid(),
            UserId = adminUserId,
            OrgId = org.Id,
            Role = UserRole.Admin,
            Status = MembershipStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        _db.Organisations.Add(org);
        _db.OrgMemberships.Add(membership);
        await _db.SaveChangesAsync();

        return org;
    }

    public async Task AddMemberAsync(Guid orgId, AddMemberRequest request)
    {
        if (_tenant.OrgId != orgId)
            throw new UnauthorizedAccessException("You can only add members to your own agency.");

        var email = request.Email.ToLower().Trim();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new InvalidOperationException("No registered user found with that email. Ask them to register first.");

        var alreadyMember = await _db.OrgMemberships
            .AnyAsync(m => m.UserId == user.Id && m.OrgId == orgId);

        if (alreadyMember)
            throw new InvalidOperationException("This user is already a member of your agency.");

        var membership = new OrgMembership
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            OrgId = orgId,
            Role = request.Role,
            Status = MembershipStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        _db.OrgMemberships.Add(membership);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateMemberRoleAsync(Guid orgId, Guid userId, UpdateMemberRoleRequest request)
    {
        if (_tenant.OrgId != orgId)
            throw new UnauthorizedAccessException("You can only update members in your own agency.");

        var membership = await _db.OrgMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.OrgId == orgId)
            ?? throw new InvalidOperationException("User is not a member of this agency.");

        // prevent removing the last Admin — every agency must have at least one
        if (membership.Role == UserRole.Admin && request.Role != UserRole.Admin)
        {
            var otherAdmins = await _db.OrgMemberships
                .CountAsync(m => m.OrgId == orgId
                              && m.Role == UserRole.Admin
                              && m.Status == MembershipStatus.Active
                              && m.UserId != userId);

            if (otherAdmins == 0)
                throw new InvalidOperationException("Cannot demote the last Admin. Promote another member to Admin first.");
        }

        membership.Role = request.Role;
        await _db.SaveChangesAsync();
    }

    public async Task<Organisation> GetMyAsync()
    {
        var orgId = _tenant.OrgId
            ?? throw new UnauthorizedAccessException("You must select a workspace first.");

        return await _db.Organisations.FirstOrDefaultAsync(o => o.Id == orgId)
            ?? throw new InvalidOperationException("Agency not found.");
    }

    public async Task<List<MemberDto>> GetMembersAsync(Guid orgId, UserRole? role)
    {
        if (_tenant.OrgId != orgId)
            throw new UnauthorizedAccessException("You can only view members of your own agency.");

        var query = _db.OrgMemberships
            .Include(m => m.User)
            .Where(m => m.OrgId == orgId);

        if (role.HasValue)
            query = query.Where(m => m.Role == role.Value);

        return await query
            .OrderBy(m => m.User.FullName)
            .Select(m => new MemberDto(
                m.UserId,
                m.User.FullName,
                m.User.Email,
                m.User.Phone,
                m.Role.ToString(),
                m.Status.ToString(),
                m.JoinedAt))
            .ToListAsync();
    }

    public async Task<MemberDetailDto> GetMemberByIdAsync(Guid orgId, Guid userId)
    {
        if (_tenant.OrgId != orgId)
            throw new UnauthorizedAccessException("You can only view members of your own agency.");

        var membership = await _db.OrgMemberships
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.OrgId == orgId && m.UserId == userId)
            ?? throw new InvalidOperationException("User is not a member of this agency.");

        var recentShifts = await _db.ShiftAssignments
            .Include(a => a.Shift)
                .ThenInclude(s => s.Site)
                    .ThenInclude(s => s.Client)
            .Include(a => a.Timesheet)
            .Where(a => a.UserId == userId && a.Shift.OrgId == orgId)
            .OrderByDescending(a => a.Shift.StartTime)
            .Take(10)
            .Select(a => new MemberShiftDto(
                a.Id,
                a.ShiftId,
                a.Shift.Site.Name,
                a.Shift.Site.Client.Name,
                a.Shift.StartTime,
                a.Shift.EndTime,
                a.Status.ToString(),
                a.Shift.Status.ToString(),
                a.Timesheet != null ? a.Timesheet.ClockIn : (DateTime?)null,
                a.Timesheet != null ? a.Timesheet.ClockOut : null,
                a.Timesheet != null ? (decimal?)a.Timesheet.HoursWorked : null))
            .ToListAsync();

        return new MemberDetailDto(
            membership.UserId,
            membership.User.FullName,
            membership.User.Email,
            membership.User.Phone,
            membership.Role.ToString(),
            membership.Status.ToString(),
            membership.JoinedAt,
            recentShifts);
    }
}
