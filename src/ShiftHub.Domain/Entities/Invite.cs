using ShiftHub.Domain.Enums;

namespace ShiftHub.Domain.Entities;

public class Invite
{
    public Guid Id { get; set; }
    public Guid OrgId { get; set; }
    public string Code { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public Guid? UsedById { get; set; }
    public DateTime? UsedAt { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }

    public Organisation Organisation { get; set; } = null!;
}
