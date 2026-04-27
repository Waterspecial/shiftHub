using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.PayRates;
using ShiftHub.Domain.Entities;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.Infrastructure.Services.PayRates;

public class PayRateService : IPayRateService
{
    private readonly ShiftHubDbContext _db;
    private readonly ICurrentTenantService _tenant;

    public PayRateService(ShiftHubDbContext db, ICurrentTenantService tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<PayRate> CreateAsync(CreatePayRateRequest request)
    {
        var payRate = new PayRate
        {
            Id = Guid.NewGuid(),
            OrgId = _tenant.OrgId!.Value,
            Name = request.Name,
            HourlyRate = request.HourlyRate,
            OvertimeRate = request.OvertimeRate,
            HolidayRate = request.HolidayRate,
            NightRate = request.NightRate
        };

        _db.PayRates.Add(payRate);
        await _db.SaveChangesAsync();

        return payRate;
    }

    public async Task<List<PayRate>> GetAllAsync()
    {
        return await _db.PayRates.ToListAsync();
    }
}
