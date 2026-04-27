using ShiftHub.Application.PayRates;
using ShiftHub.Domain.Entities;

namespace ShiftHub.Application.Interfaces;

public interface IPayRateService
{
    Task<PayRate> CreateAsync(CreatePayRateRequest request);
    Task<List<PayRate>> GetAllAsync();
}
