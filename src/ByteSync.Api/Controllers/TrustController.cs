using ByteSync.Api.Helpers;
using ByteSync.Common.Business.Sessions.Cloud.Connections;
using ByteSync.Common.Business.Trust.Connections;
using ByteSync.ServerCommon.Commands.Trusts;
using ByteSync.ServerCommon.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/trust")]
public class TrustController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;

    public TrustController(IMediator mediator, IClientsRepository clientsRepository)
    {
        _mediator = mediator;
        _clientsRepository = clientsRepository;
    }
    
    [HttpPost("startTrustCheck")]
    public async Task<IActionResult> StartTrustCheck([FromBody] TrustCheckParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new StartTrustCheckRequest(parameters, client);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    
    [HttpPost("giveMemberPublicKeyCheckData")]
    public async Task<IActionResult> GiveMemberPublicKeyCheckData([FromBody] GiveMemberPublicKeyCheckDataParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new GiveMemberPublicKeyCheckDataRequest(parameters, client);
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("sendDigitalSignatures")]
    public async Task<IActionResult> SendDigitalSignatures([FromBody] SendDigitalSignaturesParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new SendDigitalSignaturesRequest(parameters, client);
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("setAuthChecked")]
    public async Task<IActionResult> SetAuthChecked([FromBody] SetAuthCheckedParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new SetAuthCheckedRequest(parameters, client);
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("requestTrustPublicKey")]
    public async Task<IActionResult> RequestTrustPublicKey([FromBody] RequestTrustProcessParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new RequestTrustPublicKeyRequest(parameters, client);
        await _mediator.Send(request);
        return Ok();
    }
        
    [HttpPost("informPublicKeyValidationIsFinished")]
    public async Task<IActionResult> InformPublicKeyValidationIsFinished([FromBody] PublicKeyValidationParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new InformPublicKeyValidationIsFinishedRequest(parameters, client);
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("informProtocolVersionIncompatible")]
    public async Task<IActionResult> InformProtocolVersionIncompatible([FromBody] InformProtocolVersionIncompatibleParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new InformProtocolVersionIncompatibleRequest(parameters, client);
        await _mediator.Send(request);
        return Ok();
    }
}
