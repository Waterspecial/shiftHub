using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Invites;
using ShiftHub.Application.Organisations;

namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/organisations")]
[Authorize]
public class OrganisationController : ControllerBase
{
    private readonly IOrganisationService _organisationService;
    private readonly IInviteService _inviteService;

    public OrganisationController(IOrganisationService organisationService, IInviteService inviteService)
    {
        _organisationService = organisationService;
        _inviteService = inviteService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganisationRequest request)
    {
        var org = await _organisationService.CreateAsync(request);
        return Ok(new { orgId = org.Id, name = org.Name, subdomain = org.Subdomain });
    }

    [HttpPost("{orgId}/members")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> AddMember(Guid orgId, [FromBody] AddMemberRequest request)
    {
        await _organisationService.AddMemberAsync(orgId, request);
        return Ok(new { message = "Member added successfully." });
    }

    [HttpPatch("{orgId}/members/{userId}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMemberRole(Guid orgId, Guid userId, [FromBody] UpdateMemberRoleRequest request)
    {
        await _organisationService.UpdateMemberRoleAsync(orgId, userId, request);
        return Ok(new { message = "Role updated successfully." });
    }

    [HttpPost("{orgId}/invites")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateInvite(Guid orgId, [FromBody] CreateInviteRequest request)
    {
        var invite = await _inviteService.CreateAsync(orgId, request);
        return Ok(new
        {
            inviteId = invite.Id,
            code = invite.Code,
            role = invite.Role.ToString(),
            expiresAt = invite.ExpiresAt
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMy()
    {
        var org = await _organisationService.GetMyAsync();
        return Ok(new
        {
            orgId = org.Id,
            name = org.Name,
            subdomain = org.Subdomain,
            billingEmail = org.BillingEmail,
            payFrequency = org.PayFrequency.ToString(),
            createdAt = org.CreatedAt
        });
    }

    [HttpGet("{orgId}/members")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetMembers(Guid orgId)
    {
        var members = await _organisationService.GetMembersAsync(orgId);
        return Ok(members.Select(m => new
        {
            userId = m.UserId,
            fullName = m.User.FullName,
            email = m.User.Email,
            phone = m.User.Phone,
            role = m.Role.ToString(),
            status = m.Status.ToString(),
            joinedAt = m.JoinedAt,
            lastActiveAt = m.LastActiveAt
        }));
    }
}
