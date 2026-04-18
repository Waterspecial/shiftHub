namespace ShiftHub.Domain.Entities;

public class PayRate
{
    public Guid Id { get; set; }
    public Guid OrgId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal OvertimeRate { get; set; }
    public decimal HolidayRate { get; set; }
    public decimal? NightRate { get; set; }

    public Organisation Organisation { get; set; } = null!;
    public ICollection<Shift> Shifts { get; set; } = [];
}
