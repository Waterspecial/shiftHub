namespace ShiftHub.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string[] Qualifications { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public string? DeviceToken { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<OrgMembership> Memberships { get; set; } = [];
    public ICollection<ShiftAssignment> Assignments { get; set; } = [];
}
