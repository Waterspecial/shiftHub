using ShiftHub.Domain.Enums;

namespace ShiftHub.Application.Organisations;

public class CreateOrganisationRequest
{
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string BillingEmail { get; set; } = string.Empty;
    public PayFrequency PayFrequency { get; set; }
    public Guid AdminUserId { get; set; }
}
