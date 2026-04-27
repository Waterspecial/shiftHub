using ShiftHub.Domain.Enums;

namespace ShiftHub.Application.Organisations;

public class AddMemberRequest
{
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
