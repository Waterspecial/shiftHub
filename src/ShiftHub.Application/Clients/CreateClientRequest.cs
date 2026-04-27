namespace ShiftHub.Application.Clients;

public class CreateClientRequest
{
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string BillingAddress { get; set; } = string.Empty;
}
