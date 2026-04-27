namespace ShiftHub.Application.Clients;

public class CreateSiteRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
