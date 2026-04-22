namespace ShiftHub.Application.Interfaces;

public interface ICurrentTenantService
{
    Guid? OrgId { get; }
    Guid? UserId { get; }
}
