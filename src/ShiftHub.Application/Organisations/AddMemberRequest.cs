using ShiftHub.Domain.Enums;

namespace ShiftHub.Application.Organisations;

public class AddMemberRequest
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }
}
