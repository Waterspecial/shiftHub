using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShiftHub.Application.Auth;
using ShiftHub.Application.Interfaces;
using ShiftHub.Domain.Entities;
using ShiftHub.Domain.Enums;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ShiftHubDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(ShiftHubDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            throw new InvalidOperationException("An account with this email already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email.ToLower().Trim(),
            Phone = request.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResult
        {
            UserId = user.Id,
            RequiresWorkspacePicker = false,
            Token = null,
            Workspaces = []
        };
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .Include(u => u.Memberships)
                .ThenInclude(m => m.Organisation)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("This account has been deactivated.");

        var activeMemberships = user.Memberships
            .Where(m => m.Status == MembershipStatus.Active)
            .ToList();

        // 0 orgs — issue a bootstrap token so the user can create their first agency
        if (activeMemberships.Count == 0)
        {
            return new AuthResult
            {
                UserId = user.Id,
                RequiresWorkspacePicker = false,
                Token = GenerateToken(user.Id)
            };
        }

        // 1 org — issue a scoped token immediately
        if (activeMemberships.Count == 1)
        {
            var membership = activeMemberships[0];
            return new AuthResult
            {
                UserId = user.Id,
                RequiresWorkspacePicker = false,
                Token = GenerateToken(user.Id, membership.OrgId, membership.Role)
            };
        }

        // 2+ orgs — issue a bootstrap token plus workspace list so the user can pick
        return new AuthResult
        {
            UserId = user.Id,
            RequiresWorkspacePicker = true,
            Token = GenerateToken(user.Id),
            Workspaces = activeMemberships.Select(m => new WorkspaceOption
            {
                OrgId = m.OrgId,
                OrgName = m.Organisation.Name,
                Role = m.Role.ToString()
            }).ToList()
        };
    }

    public async Task<string> SelectWorkspaceAsync(Guid userId, Guid orgId)
    {
        var membership = await _db.OrgMemberships
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(m => m.UserId == userId && m.OrgId == orgId && m.Status == MembershipStatus.Active);

        if (membership == null)
            throw new UnauthorizedAccessException("You do not have access to this workspace.");

        membership.LastActiveAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return GenerateToken(userId, orgId, membership.Role);
    }

    private string GenerateToken(Guid userId, Guid? orgId = null, UserRole? role = null)
    {
        var secret = _config["Jwt__Secret"] ?? _config["Jwt:Secret"]
            ?? throw new InvalidOperationException("JWT secret is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (orgId.HasValue)
            claims.Add(new Claim("orgId", orgId.Value.ToString()));

        if (role.HasValue)
            claims.Add(new Claim(ClaimTypes.Role, role.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt__Issuer"] ?? _config["Jwt:Issuer"],
            audience: _config["Jwt__Audience"] ?? _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
