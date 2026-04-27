using ShiftHub.Application.Shifts;
using ShiftHub.Domain.Entities;

namespace ShiftHub.Application.Interfaces;

public interface IShiftService
{
    Task<Shift> CreateAsync(CreateShiftRequest request);
    Task<Shift> PublishAsync(Guid shiftId);
    Task<List<Shift>> GetAvailableAsync();
    Task<ShiftAssignment> AcceptAsync(Guid shiftId);
    Task ClockInAsync(Guid shiftId);
    Task ClockOutAsync(Guid shiftId);
}
