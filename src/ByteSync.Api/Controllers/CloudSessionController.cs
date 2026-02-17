using ByteSync.Api.Helpers;
using ByteSync.Common.Business.Sessions;
using ByteSync.Common.Business.Sessions.Cloud.Connections;
using ByteSync.ServerCommon.Commands.CloudSessions;
using ByteSync.ServerCommon.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/session")]
public class CloudSessionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;

    public CloudSessionController(IMediator mediator, IClientsRepository clientsRepository)
    {
        _mediator = mediator;
        _clientsRepository = clientsRepository;
    }
        
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCloudSessionParameters createCloudSessionParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new CreateSessionRequest(createCloudSessionParameters, client);
        var cloudSessionResult = await _mediator.Send(request);
        return Ok(cloudSessionResult);
    }
    
    [HttpPost("{sessionId}/askPasswordExchangeKey")]
    public async Task<IActionResult> AskPasswordExchangeKey(string sessionId, 
        [FromBody] AskCloudSessionPasswordExchangeKeyParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var result = await _mediator.Send(new AskPasswordExchangeKeyRequest(client, parameters), HttpContext.RequestAborted);
        return Ok(result);
    }
    
    [HttpPost("{sessionId}/validateJoin")]
    public async Task<IActionResult> ValidateJoinCloudSession(string sessionId, 
        [FromBody] ValidateJoinCloudSessionParameters parameters)
    {
        await _mediator.Send(new ValidateJoinCloudSessionRequest(parameters));
        return Ok();
    }
    
    [HttpPost("{sessionId}/finalizeJoin")]
    public async Task<IActionResult> FinalizeJoinCloudSession(string sessionId, 
        [FromBody] FinalizeJoinCloudSessionParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new FinalizeJoinCloudSessionRequest(parameters, client);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    
    [HttpPost("{sessionId}/askJoin")]
    public async Task<IActionResult> AskJoinCloudSession(string sessionId, 
        [FromBody] AskJoinCloudSessionParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var result = await _mediator.Send(new AskJoinCloudSessionRequest(client, parameters));
        return Ok(result);
    }
    
    [HttpPost("{sessionId}/givePassworkExchangeKey")]
    public async Task<IActionResult> GiveCloudSessionPasswordExchangeKey(string sessionId, 
        [FromBody] GiveCloudSessionPasswordExchangeKeyParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        await _mediator.Send(new GiveCloudSessionPasswordExchangeKeyRequest(client, parameters));
        return Ok();
    }
    
    [HttpPost("{sessionId}/informPasswordIsWrong")]
    public async Task<IActionResult> InformPasswordIsWrong(string sessionId, [FromBody] string clientInstanceId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        await _mediator.Send(new InformPasswordIsWrongRequest(client, sessionId, clientInstanceId));
        return Ok();
    }
    
    [HttpPost("{sessionId}/updateSettings")]
    public async Task<IActionResult> UpdateSettings(string sessionId, [FromBody] EncryptedSessionSettings settings)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new UpdateSessionSettingsRequest(sessionId, client, settings);
        var settingsUpdated = await _mediator.Send(request);
        
        if (settingsUpdated)
        {
            return Ok();
        }
        
        return Conflict();
    }
    
    [HttpPost("{sessionId}/quit")]
    public async Task<IActionResult> Quit(string sessionId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new QuitSessionRequest(sessionId, client);
        await _mediator.Send(request);
        return Ok();
    }
        
    [HttpPost("{sessionId}/reset")]
    public async Task<IActionResult> Reset(string sessionId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        await _mediator.Send(new ResetSessionRequest(sessionId, client));
        return Ok();
    }
}
