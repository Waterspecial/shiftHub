using ShiftHub.Application.Clients;
using ShiftHub.Domain.Entities;

namespace ShiftHub.Application.Interfaces;

public interface IClientService
{
    Task<Client> CreateAsync(CreateClientRequest request);
    Task<List<Client>> GetAllAsync();
    Task<Client> GetByIdAsync(Guid clientId);
    Task<Site> CreateSiteAsync(Guid clientId, CreateSiteRequest request);
    Task<List<Site>> GetSitesAsync(Guid clientId);
}
