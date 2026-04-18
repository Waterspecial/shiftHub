namespace ShiftHub.Domain.Entities;

public class Site
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public Client Client { get; set; } = null!;
    public ICollection<Shift> Shifts { get; set; } = [];
}
