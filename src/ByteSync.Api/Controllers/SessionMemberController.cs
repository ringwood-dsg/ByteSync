using ByteSync.Api.Helpers;
using ByteSync.Common.Business.Sessions.Cloud;
using ByteSync.ServerCommon.Commands.SessionMembers;
using ByteSync.ServerCommon.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/session/{sessionId}/members")]
public class SessionMemberController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;

    public SessionMemberController(IMediator mediator, IClientsRepository clientsRepository)
    {
        _mediator = mediator;
        _clientsRepository = clientsRepository;
    }
    
    [HttpGet("InstanceIds")]
    public async Task<IActionResult> GetMembersInstanceIds(string sessionId)
    {
        var result = await _mediator.Send(new GetMembersInstanceIdsRequest(sessionId));
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetMembers(string sessionId)
    {
        var result = await _mediator.Send(new GetMembersRequest(sessionId));
        return Ok(result);
    }

    [HttpPost("{clientInstanceId}/generalStatus")]
    public async Task<IActionResult> SetGeneralStatus(string sessionId, 
        [FromBody] UpdateSessionMemberGeneralStatusParameters generalStatusParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new SetGeneralStatusRequest(client, generalStatusParameters);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
