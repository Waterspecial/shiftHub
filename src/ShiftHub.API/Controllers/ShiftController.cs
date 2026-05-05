using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Shifts;
using ShiftHub.Domain.Enums;

namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/shifts")]
[Authorize]
public class ShiftController : ControllerBase
{
    private readonly IShiftService _shiftService;

    public ShiftController(IShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateShiftRequest request)
    {
        var shift = await _shiftService.CreateAsync(request);
        return Ok(new
        {
            shiftId = shift.Id,
            startTime = shift.StartTime,
            endTime = shift.EndTime,
            breakMinutes = shift.BreakMinutes,
            status = shift.Status.ToString()
        });
    }

    [HttpPost("{shiftId}/publish")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Publish(Guid shiftId)
    {
        var shift = await _shiftService.PublishAsync(shiftId);
        return Ok(new { shiftId = shift.Id, status = shift.Status.ToString() });
    }

    [HttpPost("{shiftId}/assign")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Assign(Guid shiftId, [FromBody] AssignShiftRequest request)
    {
        var assignment = await _shiftService.AssignAsync(shiftId, request.UserId);
        return Ok(new
        {
            assignmentId = assignment.Id,
            shiftId = assignment.ShiftId,
            userId = assignment.UserId,
            status = assignment.Status.ToString()
        });
    }

    [HttpPost("{shiftId}/open")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Open(Guid shiftId)
    {
        var shift = await _shiftService.OpenAsync(shiftId);
        return Ok(new { shiftId = shift.Id, status = shift.Status.ToString() });
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var shifts = await _shiftService.GetAvailableAsync();
        return Ok(shifts.Select(s => new
        {
            shiftId = s.Id,
            site = s.Site.Name,
            address = s.Site.Address,
            postcode = s.Site.Postcode,
            startTime = s.StartTime,
            endTime = s.EndTime,
            breakMinutes = s.BreakMinutes,
            payRate = s.PayRate.Name,
            hourlyRate = s.PayRate.HourlyRate,
            slotsNeeded = s.SlotsNeeded,
            slotsRemaining = s.SlotsNeeded - s.Assignments.Count(a => a.Status == AssignmentStatus.Accepted),
            notes = s.Notes
        }));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAll([FromQuery] ShiftStatus? status)
    {
        var shifts = await _shiftService.GetAllAsync(status);
        return Ok(shifts.Select(s => new
        {
            shiftId = s.Id,
            site = s.Site.Name,
            client = s.Site.Client.Name,
            startTime = s.StartTime,
            endTime = s.EndTime,
            breakMinutes = s.BreakMinutes,
            payRate = s.PayRate.Name,
            hourlyRate = s.PayRate.HourlyRate,
            slotsNeeded = s.SlotsNeeded,
            slotsFilled = s.Assignments.Count(a => a.Status == AssignmentStatus.Accepted),
            status = s.Status.ToString(),
            notes = s.Notes
        }));
    }

    [HttpGet("{shiftId}")]
    public async Task<IActionResult> GetById(Guid shiftId)
    {
        var s = await _shiftService.GetByIdAsync(shiftId);
        return Ok(new
        {
            shiftId = s.Id,
            site = new { siteId = s.Site.Id, name = s.Site.Name, address = s.Site.Address, postcode = s.Site.Postcode },
            client = new { clientId = s.Site.Client.Id, name = s.Site.Client.Name },
            startTime = s.StartTime,
            endTime = s.EndTime,
            breakMinutes = s.BreakMinutes,
            payRate = new { name = s.PayRate.Name, hourlyRate = s.PayRate.HourlyRate },
            slotsNeeded = s.SlotsNeeded,
            status = s.Status.ToString(),
            notes = s.Notes,
            assignments = s.Assignments.Select(a => new
            {
                assignmentId = a.Id,
                userId = a.UserId,
                fullName = a.User.FullName,
                email = a.User.Email,
                status = a.Status.ToString(),
                acceptedAt = a.AcceptedAt,
                clockIn = a.Timesheet != null ? a.Timesheet.ClockIn : (DateTime?)null,
                clockOut = a.Timesheet != null ? a.Timesheet.ClockOut : null,
                hoursWorked = a.Timesheet != null ? (decimal?)a.Timesheet.HoursWorked : null
            })
        });
    }

    [HttpGet("my")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> GetMy()
    {
        var assignments = await _shiftService.GetMyShiftsAsync();
        return Ok(assignments.Select(a => new
        {
            assignmentId = a.Id,
            shiftId = a.ShiftId,
            site = a.Shift.Site.Name,
            client = a.Shift.Site.Client.Name,
            address = a.Shift.Site.Address,
            postcode = a.Shift.Site.Postcode,
            startTime = a.Shift.StartTime,
            endTime = a.Shift.EndTime,
            breakMinutes = a.Shift.BreakMinutes,
            payRate = a.Shift.PayRate.Name,
            hourlyRate = a.Shift.PayRate.HourlyRate,
            assignmentStatus = a.Status.ToString(),
            shiftStatus = a.Shift.Status.ToString(),
            clockIn = a.Timesheet != null ? a.Timesheet.ClockIn : (DateTime?)null,
            clockOut = a.Timesheet != null ? a.Timesheet.ClockOut : null,
            hoursWorked = a.Timesheet != null ? (decimal?)a.Timesheet.HoursWorked : null
        }));
    }

    [HttpGet("my/timesheets")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> GetMyTimesheets()
    {
        var timesheets = await _shiftService.GetMyTimesheetsAsync();
        return Ok(timesheets.Select(t => new
        {
            timesheetId = t.Id,
            shiftId = t.Assignment.ShiftId,
            site = t.Assignment.Shift.Site.Name,
            startTime = t.Assignment.Shift.StartTime,
            endTime = t.Assignment.Shift.EndTime,
            clockIn = t.ClockIn,
            clockOut = t.ClockOut,
            hoursWorked = t.HoursWorked,
            hourlyRate = t.Assignment.Shift.PayRate.HourlyRate,
            estimatedPay = t.ClockOut.HasValue
                ? Math.Round(t.HoursWorked * t.Assignment.Shift.PayRate.HourlyRate, 2)
                : (decimal?)null
        }));
    }

    [HttpPost("{shiftId}/accept")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> Accept(Guid shiftId)
    {
        var assignment = await _shiftService.AcceptAsync(shiftId);
        return Ok(new { assignmentId = assignment.Id, status = assignment.Status.ToString() });
    }

    [HttpPost("{shiftId}/clock-in")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> ClockIn(Guid shiftId)
    {
        await _shiftService.ClockInAsync(shiftId);
        return Ok(new { message = "Clocked in successfully.", time = DateTime.UtcNow });
    }

    [HttpPost("{shiftId}/clock-out")]
    [Authorize(Roles = "Worker")]
    public async Task<IActionResult> ClockOut(Guid shiftId)
    {
        await _shiftService.ClockOutAsync(shiftId);
        return Ok(new { message = "Clocked out successfully.", time = DateTime.UtcNow });
    }
}
