using ShiftHub.Domain.Enums;

namespace ShiftHub.Domain.Entities;

public class ShiftAssignment
{
    public Guid Id { get; set; }
    public Guid ShiftId { get; set; }
    public Guid UserId { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }

    public Shift Shift { get; set; } = null!;
    public User User { get; set; } = null!;
    public Timesheet? Timesheet { get; set; }
}
