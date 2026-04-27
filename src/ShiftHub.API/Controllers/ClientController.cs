using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShiftHub.Application.Clients;
using ShiftHub.Application.Interfaces;


namespace ShiftHub.API.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize]
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
        return Ok(clients);
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
