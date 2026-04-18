namespace ShiftHub.Domain.Entities;

public class Timesheet
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public DateTime ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public decimal HoursWorked { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ApprovedById { get; set; }
    public DateTime? ApprovedAt { get; set; }

    public ShiftAssignment Assignment { get; set; } = null!;
}
