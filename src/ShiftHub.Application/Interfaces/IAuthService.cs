using ShiftHub.Application.Auth;

namespace ShiftHub.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> SelectWorkspaceAsync(Guid userId, Guid orgId);
}
