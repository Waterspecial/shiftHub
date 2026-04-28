using ShiftHub.Domain.Enums;

namespace ShiftHub.Application.Organisations;

public class UpdateMemberRoleRequest
{
    public UserRole Role { get; set; }
}
