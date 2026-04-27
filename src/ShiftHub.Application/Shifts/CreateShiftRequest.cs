namespace ShiftHub.Application.Shifts;

public class CreateShiftRequest
{
    public Guid SiteId { get; set; }
    public Guid PayRateId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int SlotsNeeded { get; set; }
    public int? BreakMinutes { get; set; }
    public string? Notes { get; set; }
}
