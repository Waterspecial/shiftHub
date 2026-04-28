using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Organisations;
using ShiftHub.Domain.Entities;
using ShiftHub.Domain.Enums;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.Infrastructure.Services.Organisations;

public class OrganisationService : IOrganisationService
{
    private readonly ShiftHubDbContext _db;
    private readonly ICurrentTenantService _tenant;

    public OrganisationService(ShiftHubDbContext db, ICurrentTenantService tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<Organisation> CreateAsync(CreateOrganisationRequest request)
    {
        var adminUserId = _tenant.UserId
            ?? throw new UnauthorizedAccessException("You must be logged in to create an agency.");

        var subdomainTaken = await _db.Organisations
            .AnyAsync(o => o.Subdomain == request.Subdomain.ToLower().Trim());

        if (subdomainTaken)
            throw new InvalidOperationException("This subdomain is already taken.");

        var org = new Organisation
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Subdomain = request.Subdomain.ToLower().Trim(),
            BillingEmail = request.BillingEmail,
            PayFrequency = request.PayFrequency,
            CreatedAt = DateTime.UtcNow
        };

        var membership = new OrgMembership
        {
            Id = Guid.NewGuid(),
            UserId = adminUserId,
            OrgId = org.Id,
            Role = UserRole.Admin,
            Status = MembershipStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        _db.Organisations.Add(org);
        _db.OrgMemberships.Add(membership);
        await _db.SaveChangesAsync();

        return org;
    }

    public async Task AddMemberAsync(Guid orgId, AddMemberRequest request)
    {
        if (_tenant.OrgId != orgId)
            throw new UnauthorizedAccessException("You can only add members to your own agency.");

        var email = request.Email.ToLower().Trim();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new InvalidOperationException("No registered user found with that email. Ask them to register first.");

        var alreadyMember = await _db.OrgMemberships
            .AnyAsync(m => m.UserId == user.Id && m.OrgId == orgId);

        if (alreadyMember)
            throw new InvalidOperationException("This user is already a member of your agency.");

        var membership = new OrgMembership
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            OrgId = orgId,
            Role = request.Role,
            Status = MembershipStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        _db.OrgMemberships.Add(membership);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateMemberRoleAsync(Guid orgId, Guid userId, UpdateMemberRoleRequest request)
    {
        if (_tenant.OrgId != orgId)
            throw new UnauthorizedAccessException("You can only update members in your own agency.");

        var membership = await _db.OrgMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.OrgId == orgId)
            ?? throw new InvalidOperationException("User is not a member of this agency.");

        // prevent removing the last Admin — every agency must have at least one
        if (membership.Role == UserRole.Admin && request.Role != UserRole.Admin)
        {
            var otherAdmins = await _db.OrgMemberships
                .CountAsync(m => m.OrgId == orgId
                              && m.Role == UserRole.Admin
                              && m.Status == MembershipStatus.Active
                              && m.UserId != userId);

            if (otherAdmins == 0)
                throw new InvalidOperationException("Cannot demote the last Admin. Promote another member to Admin first.");
        }

        membership.Role = request.Role;
        await _db.SaveChangesAsync();
    }
}
