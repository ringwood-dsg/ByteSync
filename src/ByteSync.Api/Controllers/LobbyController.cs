using ByteSync.Api.Helpers;
using ByteSync.Common.Business.Lobbies;
using ByteSync.Common.Business.Lobbies.Connections;
using ByteSync.ServerCommon.Commands.Lobbies;
using ByteSync.ServerCommon.Interfaces.Repositories;
using ByteSync.ServerCommon.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LobbyController : ControllerBase
{
    private readonly ILobbyService _lobbyService;
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;

    public LobbyController(ILobbyService lobbyService, IMediator mediator, IClientsRepository clientsRepository)
    {
        _lobbyService = lobbyService;
        _mediator = mediator;
        _clientsRepository = clientsRepository;
    }
    
    [HttpPost("join/{cloudSessionProfileId}")]
    public async Task<IActionResult> JoinLobby(string cloudSessionProfileId, [FromBody] JoinLobbyParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new TryJoinLobbyRequest(parameters, client);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    
    [HttpPost("{lobbyId}/sendCloudSessionCredentials")]
    public async Task<IActionResult> SendLobbyCloudSessionCredentials(string lobbyId, 
        [FromBody] LobbyCloudSessionCredentials lobbyCloudSessionCredentials)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        await _lobbyService.SendLobbyCloudSessionCredentials(lobbyCloudSessionCredentials, client);
        return Ok();
    }
    
    [HttpPost("{lobbyId}/quit")]
    public async Task<IActionResult> QuitLobby(string lobbyId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new QuitLobbyRequest(lobbyId, client);
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("{lobbyId}/checkInfos")]
    public async Task<IActionResult> SendLobbyCheckInfos(string lobbyId, [FromBody] LobbyCheckInfo lobbyCheckInfo)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        await _lobbyService.SendLobbyCheckInfos(lobbyCheckInfo, client);
        return Ok();
    }
    
    [HttpPost("{lobbyId}/memberStatus")]
    public async Task<IActionResult> UpdateLobbyMemberStatus(string lobbyId, [FromBody] LobbyMemberStatuses lobbyMemberStatus)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        await _lobbyService.UpdateLobbyMemberStatus(lobbyId, client, lobbyMemberStatus);
        return Ok();
    }
}
