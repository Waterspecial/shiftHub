using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Invites;

namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/invites")]
[Authorize]
public class InviteController : ControllerBase
{
    private readonly IInviteService _inviteService;

    public InviteController(IInviteService inviteService)
    {
        _inviteService = inviteService;
    }

    [HttpPost("redeem")]
    public async Task<IActionResult> Redeem([FromBody] RedeemInviteRequest request)
    {
        var membership = await _inviteService.RedeemAsync(request);
        return Ok(new
        {
            message = "You have joined the agency. Log in again to switch into this workspace.",
            orgId = membership.OrgId,
            role = membership.Role.ToString()
        });
    }
}
