using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Clients;
using ShiftHub.Application.Interfaces;


namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize(Roles = "Admin,Manager")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        var client = await _clientService.CreateAsync(request);
        return Ok(new { clientId = client.Id, name = client.Name });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _clientService.GetAllAsync();
        return Ok(clients.Select(c => new
        {
            clientId = c.Id,
            name = c.Name,
            contactName = c.ContactName,
            contactEmail = c.ContactEmail,
            contactPhone = c.ContactPhone
        }));
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetById(Guid clientId)
    {
        var client = await _clientService.GetByIdAsync(clientId);
        return Ok(new
        {
            clientId = client.Id,
            name = client.Name,
            contactName = client.ContactName,
            contactEmail = client.ContactEmail,
            contactPhone = client.ContactPhone,
            billingAddress = client.BillingAddress,
            createdAt = client.CreatedAt,
            sites = client.Sites.Select(s => new
            {
                siteId = s.Id,
                name = s.Name,
                address = s.Address,
                postcode = s.Postcode,
                notes = s.Notes
            })
        });
    }

    [HttpPost("{clientId}/sites")]
    public async Task<IActionResult> CreateSite(Guid clientId, [FromBody] CreateSiteRequest request)
    {
        var site = await _clientService.CreateSiteAsync(clientId, request);
        return Ok(new { siteId = site.Id, name = site.Name, address = site.Address });
    }

    [HttpGet("{clientId}/sites")]
    public async Task<IActionResult> GetSites(Guid clientId)
    {
        var sites = await _clientService.GetSitesAsync(clientId);
        return Ok(sites);
    }
}
