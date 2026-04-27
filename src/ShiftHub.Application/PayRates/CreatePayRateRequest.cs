namespace ShiftHub.Application.PayRates;

public class CreatePayRateRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal OvertimeRate { get; set; }
    public decimal HolidayRate { get; set; }
    public decimal? NightRate { get; set; }
}
