using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Organisations;

namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/organisations")]
public class OrganisationController : ControllerBase
{
    private readonly IOrganisationService _organisationService;

    public OrganisationController(IOrganisationService organisationService)
    {
        _organisationService = organisationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrganisationRequest request)
    {
        var org = await _organisationService.CreateAsync(request);
        return Ok(new { orgId = org.Id, name = org.Name, subdomain = org.Subdomain });
    }

    [HttpPost("{orgId}/members")]
    public async Task<IActionResult> AddMember(Guid orgId, [FromBody] AddMemberRequest request)
    {
        await _organisationService.AddMemberAsync(orgId, request);
        return Ok(new { message = "Member added successfully." });
    }
}
