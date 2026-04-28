using ShiftHub.Application.Invites;
using ShiftHub.Domain.Entities;

namespace ShiftHub.Application.Interfaces;

public interface IInviteService
{
    Task<Invite> CreateAsync(Guid orgId, CreateInviteRequest request);
    Task<OrgMembership> RedeemAsync(RedeemInviteRequest request);
}
