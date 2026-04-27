using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Shifts;

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
            slotsRemaining = s.SlotsNeeded - s.Assignments.Count(a => a.Status == Domain.Enums.AssignmentStatus.Accepted),
            notes = s.Notes
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
