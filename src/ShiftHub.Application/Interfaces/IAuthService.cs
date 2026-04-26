using ShiftHub.Application.Auth;

namespace ShiftHub.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<string> SelectWorkspaceAsync(Guid userId, Guid orgId);
}
