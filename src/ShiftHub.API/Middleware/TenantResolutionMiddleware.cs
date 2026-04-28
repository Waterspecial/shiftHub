using Microsoft.EntityFrameworkCore;
using ShiftHub.Domain.Enums;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.API.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ShiftHubDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var orgClaim = context.User.FindFirst("orgId")?.Value;
            var userClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                            ?? context.User.FindFirst("sub")?.Value;

            if (orgClaim != null && userClaim != null)
            {
                var orgId = Guid.Parse(orgClaim);
                var userId = Guid.Parse(userClaim);

                var hasActiveMembership = await db.OrgMemberships
                    .AnyAsync(m => m.UserId == userId
                                && m.OrgId == orgId
                                && m.Status == MembershipStatus.Active);

                if (!hasActiveMembership)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "You are no longer a member of this workspace."
                    });
                    return;
                }
            }
        }

        await _next(context);
    }
}
