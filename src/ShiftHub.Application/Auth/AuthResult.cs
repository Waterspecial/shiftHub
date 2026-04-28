namespace ShiftHub.Application.Auth;

public class AuthResult
{
    public string? Token { get; set; }
    public UserInfo User { get; set; } = new();
    public WorkspaceOption? CurrentWorkspace { get; set; }
    public bool RequiresWorkspacePicker { get; set; }
    public List<WorkspaceOption> Workspaces { get; set; } = [];
}

public class UserInfo
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class WorkspaceOption
{
    public Guid OrgId { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
