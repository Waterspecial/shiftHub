using ShiftHub.Domain.Enums;

namespace ShiftHub.Domain.Entities;

public class Shift
{
    public Guid Id { get; set; }
    public Guid OrgId { get; set; }
    public Guid SiteId { get; set; }
    public Guid PayRateId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int SlotsNeeded { get; set; }
    public int BreakMinutes { get; set; }
    public ShiftStatus Status { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }

    public Organisation Organisation { get; set; } = null!;
    public Site Site { get; set; } = null!;
    public PayRate PayRate { get; set; } = null!;
    public ICollection<ShiftAssignment> Assignments { get; set; } = [];
}
