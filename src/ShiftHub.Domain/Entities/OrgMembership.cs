using ShiftHub.Domain.Enums;

namespace ShiftHub.Domain.Entities;

public class OrgMembership
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid OrgId { get; set; }
    public UserRole Role { get; set; }
    public MembershipStatus Status { get; set; }
    public DateTime JoinedAt { get; set; }
    public Guid? InvitedById { get; set; }
    public DateTime? LastActiveAt { get; set; }

    public User User { get; set; } = null!;
    public Organisation Organisation { get; set; } = null!;
}
