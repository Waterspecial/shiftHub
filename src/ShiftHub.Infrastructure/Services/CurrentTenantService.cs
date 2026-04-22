using Microsoft.AspNetCore.Http;
using ShiftHub.Application.Interfaces;

namespace ShiftHub.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid? OrgId { get; }
    public Guid? UserId { get; }

    public CurrentTenantService(IHttpContextAccessor httpContext)
    {
        var orgClaim = httpContext.HttpContext?.User?.FindFirst("orgId");
        if (orgClaim != null) OrgId = Guid.Parse(orgClaim.Value);

        var userClaim = httpContext.HttpContext?.User?.FindFirst("sub");
        if (userClaim != null) UserId = Guid.Parse(userClaim.Value);
    }
}
