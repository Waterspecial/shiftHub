namespace ShiftHub.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public Guid OrgId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string BillingAddress { get; set; } = string.Empty;
    public decimal? HealthScore { get; set; }
    public DateTime CreatedAt { get; set; }

    public Organisation Organisation { get; set; } = null!;
    public ICollection<Site> Sites { get; set; } = [];
}
