using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Interfaces;
using ShiftHub.Application.Invites;
using ShiftHub.Domain.Entities;
using ShiftHub.Domain.Enums;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.Infrastructure.Services.Invites;

public class InviteService : IInviteService
{
    private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 8;

    private readonly ShiftHubDbContext _db;
    private readonly ICurrentTenantService _tenant;

    public InviteService(ShiftHubDbContext db, ICurrentTenantService tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<Invite> CreateAsync(Guid orgId, CreateInviteRequest request)
    {
        if (_tenant.OrgId != orgId)
            throw new UnauthorizedAccessException("You can only create invites for your own agency.");

        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            OrgId = orgId,
            Code = await GenerateUniqueCodeAsync(),
            Role = request.Role,
            ExpiresAt = DateTime.UtcNow.AddDays(request.ExpiresInDays ?? 7),
            IsUsed = false,
            CreatedById = _tenant.UserId!.Value,
            CreatedAt = DateTime.UtcNow
        };

        _db.Invites.Add(invite);
        await _db.SaveChangesAsync();

        return invite;
    }

    public async Task<OrgMembership> RedeemAsync(RedeemInviteRequest request)
    {
        var userId = _tenant.UserId
            ?? throw new UnauthorizedAccessException("You must be logged in to join an agency.");

        var code = request.Code.ToUpper().Trim();

        var invite = await _db.Invites
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.Code == code)
            ?? throw new InvalidOperationException("Invite code not found.");

        if (invite.IsUsed)
            throw new InvalidOperationException("This invite has already been used.");

        if (invite.ExpiresAt < DateTime.UtcNow)
            throw new InvalidOperationException("This invite has expired.");

        var alreadyMember = await _db.OrgMemberships
            .IgnoreQueryFilters()
            .AnyAsync(m => m.UserId == userId && m.OrgId == invite.OrgId);

        if (alreadyMember)
            throw new InvalidOperationException("You are already a member of this agency.");

        var membership = new OrgMembership
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrgId = invite.OrgId,
            Role = invite.Role,
            Status = MembershipStatus.Active,
            JoinedAt = DateTime.UtcNow
        };

        invite.IsUsed = true;
        invite.UsedById = userId;
        invite.UsedAt = DateTime.UtcNow;

        _db.OrgMemberships.Add(membership);
        await _db.SaveChangesAsync();

        return membership;
    }

    private async Task<string> GenerateUniqueCodeAsync()
    {
        for (var attempt = 0; attempt < 10; attempt++)
        {
            var code = GenerateCode();
            var exists = await _db.Invites.AnyAsync(i => i.Code == code);
            if (!exists) return code;
        }

        throw new InvalidOperationException("Failed to generate a unique invite code. Please try again.");
    }

    private static string GenerateCode()
    {
        var chars = new char[CodeLength];
        for (var i = 0; i < CodeLength; i++)
            chars[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
        return new string(chars);
    }
}
