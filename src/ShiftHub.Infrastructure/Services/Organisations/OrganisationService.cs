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

    public OrganisationService(ShiftHubDbContext db)
    {
        _db = db;
    }

    public async Task<Organisation> CreateAsync(CreateOrganisationRequest request)
    {
        var subdomainTaken = await _db.Organisations
            .AnyAsync(o => o.Subdomain == request.Subdomain.ToLower().Trim());

        if (subdomainTaken)
            throw new InvalidOperationException("This subdomain is already taken.");

        var adminExists = await _db.Users.AnyAsync(u => u.Id == request.AdminUserId);
        if (!adminExists)
            throw new InvalidOperationException("User not found.");

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
            UserId = request.AdminUserId,
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
        var orgExists = await _db.Organisations.AnyAsync(o => o.Id == orgId);
        if (!orgExists)
            throw new InvalidOperationException("Organisation not found.");

        var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
            throw new InvalidOperationException("User not found.");

        var alreadyMember = await _db.OrgMemberships
            .AnyAsync(m => m.UserId == request.UserId && m.OrgId == orgId);

        if (alreadyMember)
            throw new InvalidOperationException("User is already a member of this organisation.");

        var membership = new OrgMembership
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            OrgId = orgId,
            Role = request.Role,
            Status = MembershipStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        _db.OrgMemberships.Add(membership);
        await _db.SaveChangesAsync();
    }
}
