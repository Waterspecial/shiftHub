namespace ShiftHub.Application.Auth;

public class AuthResult
{
    public bool RequiresWorkspacePicker { get; set; }
    public string? Token { get; set; }
    public Guid UserId { get; set; }
    public List<WorkspaceOption> Workspaces { get; set; } = [];
}

public class WorkspaceOption
{
    public Guid OrgId { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
