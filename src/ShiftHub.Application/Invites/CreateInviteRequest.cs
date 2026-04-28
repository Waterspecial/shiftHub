using ShiftHub.Domain.Enums;

namespace ShiftHub.Application.Invites;

public class CreateInviteRequest
{
    public UserRole Role { get; set; }
    public int? ExpiresInDays { get; set; }
}
