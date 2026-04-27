using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShiftHub.Application.Interfaces;

namespace ShiftHub.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid? OrgId { get; }
    public Guid? UserId { get; }

    public CurrentTenantService(IHttpContextAccessor httpContext)
    {
        var user = httpContext.HttpContext?.User;

        var orgClaim = user?.FindFirst("orgId");
        if (orgClaim != null) OrgId = Guid.Parse(orgClaim.Value);

        // JWT middleware maps "sub" to ClaimTypes.NameIdentifier
        var userClaim = user?.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user?.FindFirst("sub");
        if (userClaim != null) UserId = Guid.Parse(userClaim.Value);
    }
}
