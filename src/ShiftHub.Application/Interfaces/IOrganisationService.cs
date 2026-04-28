using ShiftHub.Application.Organisations;
using ShiftHub.Domain.Entities;

namespace ShiftHub.Application.Interfaces;

public interface IOrganisationService
{
    Task<Organisation> CreateAsync(CreateOrganisationRequest request);
    Task AddMemberAsync(Guid orgId, AddMemberRequest request);
    Task UpdateMemberRoleAsync(Guid orgId, Guid userId, UpdateMemberRoleRequest request);
}
