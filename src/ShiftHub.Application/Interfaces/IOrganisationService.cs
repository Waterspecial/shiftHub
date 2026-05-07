using ShiftHub.Application.Organisations;
using ShiftHub.Domain.Entities;
using ShiftHub.Domain.Enums;

namespace ShiftHub.Application.Interfaces;

public interface IOrganisationService
{
    Task<Organisation> CreateAsync(CreateOrganisationRequest request);
    Task AddMemberAsync(Guid orgId, AddMemberRequest request);
    Task UpdateMemberRoleAsync(Guid orgId, Guid userId, UpdateMemberRoleRequest request);
    Task<Organisation> GetMyAsync();
    Task<List<MemberDto>> GetMembersAsync(Guid orgId, UserRole? role);
    Task<MemberDetailDto> GetMemberByIdAsync(Guid orgId, Guid userId);
}
