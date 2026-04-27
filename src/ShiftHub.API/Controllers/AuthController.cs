using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Auth;
using ShiftHub.Application.Interfaces;

namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        return Ok(new { message = "Account created successfully.", userId = result.UserId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(new
        {
            token = result.Token,
            userId = result.UserId,
            requiresWorkspacePicker = result.RequiresWorkspacePicker,
            workspaces = result.Workspaces
        });
    }

    [HttpPost("select-workspace")]
    public async Task<IActionResult> SelectWorkspace([FromBody] SelectWorkspaceRequest request)
    {
        var token = await _authService.SelectWorkspaceAsync(request.UserId, request.OrgId);
        return Ok(new { token });
    }
}

public class SelectWorkspaceRequest
{
    public Guid UserId { get; set; }
    public Guid OrgId { get; set; }
}
