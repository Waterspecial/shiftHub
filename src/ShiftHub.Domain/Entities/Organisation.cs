using ShiftHub.Domain.Enums;

namespace ShiftHub.Domain.Entities;

public class Organisation
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string BillingEmail { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public PayFrequency PayFrequency { get; set; }
    public string? Settings { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<OrgMembership> Memberships { get; set; } = [];
    public ICollection<Client> Clients { get; set; } = [];
    public ICollection<Shift> Shifts { get; set; } = [];
    public ICollection<PayRate> PayRates { get; set; } = [];
}
