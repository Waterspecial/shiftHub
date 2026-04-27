using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.PayRates;

namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/payrates")]
[Authorize]
public class PayRateController : ControllerBase
{
    private readonly IPayRateService _payRateService;

    public PayRateController(IPayRateService payRateService)
    {
        _payRateService = payRateService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePayRateRequest request)
    {
        var payRate = await _payRateService.CreateAsync(request);
        return Ok(new { payRateId = payRate.Id, name = payRate.Name, hourlyRate = payRate.HourlyRate });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var payRates = await _payRateService.GetAllAsync();
        return Ok(payRates);
    }
}
