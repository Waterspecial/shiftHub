using Microsoft.EntityFrameworkCore;
using ShiftHub.Application.Clients;
using ShiftHub.Application.Interfaces;
using ShiftHub.Domain.Entities;
using ShiftHub.Infrastructure.Persistence;

namespace ShiftHub.Infrastructure.Services.Clients;

public class ClientService : IClientService
{
    private readonly ShiftHubDbContext _db;
    private readonly ICurrentTenantService _tenant;

    public ClientService(ShiftHubDbContext db, ICurrentTenantService tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<Client> CreateAsync(CreateClientRequest request)
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            OrgId = _tenant.OrgId!.Value,
            Name = request.Name,
            ContactName = request.ContactName,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            BillingAddress = request.BillingAddress,
            CreatedAt = DateTime.UtcNow
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        return client;
    }

    public async Task<List<Client>> GetAllAsync()
    {
        return await _db.Clients.ToListAsync();
    }

    public async Task<Client> GetByIdAsync(Guid clientId)
    {
        return await _db.Clients
            .Include(c => c.Sites)
            .FirstOrDefaultAsync(c => c.Id == clientId)
            ?? throw new InvalidOperationException("Client not found.");
    }

    public async Task<Site> CreateSiteAsync(Guid clientId, CreateSiteRequest request)
    {
        var clientExists = await _db.Clients.AnyAsync(c => c.Id == clientId);
        if (!clientExists)
            throw new InvalidOperationException("Client not found.");

        var site = new Site
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            Name = request.Name,
            Address = request.Address,
            Postcode = request.Postcode,
            Notes = request.Notes
        };

        _db.Sites.Add(site);
        await _db.SaveChangesAsync();

        return site;
    }

    public async Task<List<Site>> GetSitesAsync(Guid clientId)
    {
        return await _db.Sites.Where(s => s.ClientId == clientId).ToListAsync();
    }
}
