using ShiftHub.Application.Shifts;
using ShiftHub.Domain.Entities;
using ShiftHub.Domain.Enums;

namespace ShiftHub.Application.Interfaces;

public interface IShiftService
{
    Task<Shift> CreateAsync(CreateShiftRequest request);
    Task<Shift> PublishAsync(Guid shiftId);
    Task<Shift> OpenAsync(Guid shiftId);
    Task<ShiftAssignment> AssignAsync(Guid shiftId, Guid userId);
    Task<List<Shift>> GetAvailableAsync();
    Task<List<Shift>> GetAllAsync(ShiftStatus? status);
    Task<Shift> GetByIdAsync(Guid shiftId);
    Task<List<ShiftAssignment>> GetMyShiftsAsync();
    Task<List<Timesheet>> GetMyTimesheetsAsync();
    Task<ShiftAssignment> AcceptAsync(Guid shiftId);
    Task ClockInAsync(Guid shiftId);
    Task ClockOutAsync(Guid shiftId);
}
