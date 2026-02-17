using ByteSync.Api.Helpers;
using ByteSync.Common.Business.Synchronizations;
using ByteSync.ServerCommon.Commands.Synchronizations;
using ByteSync.ServerCommon.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/session/{sessionId}/synchronization")]
public class SynchronizationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;

    public SynchronizationController(IMediator mediator, IClientsRepository clientsRepository)
    {
        _mediator = mediator;
        _clientsRepository = clientsRepository;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartSynchronization(string sessionId, 
        [FromBody] SynchronizationStartRequest synchronizationStartRequest)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new StartSynchronizationRequest(sessionId, client, synchronizationStartRequest.ActionsGroupDefinitions);
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("localCopyIsDone")]
    public async Task<IActionResult> LocalCopyIsDone(string sessionId, 
        [FromBody] SynchronizationActionRequest synchronizationActionRequest)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new LocalCopyIsDoneRequest(sessionId, client, synchronizationActionRequest.ActionsGroupIds,
            synchronizationActionRequest.NodeId,
            synchronizationActionRequest.ActionMetricsByActionId);
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("dateIsCopied")]
    public async Task<IActionResult> DateIsCopied(string sessionId, 
        [FromBody] SynchronizationActionRequest synchronizationActionRequest)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new DateIsCopiedRequest(sessionId, client, synchronizationActionRequest.ActionsGroupIds,
            synchronizationActionRequest.NodeId);
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("fileOrDirectoryIsDeleted")]
    public async Task<IActionResult> FileOrDirectoryIsDeleted(string sessionId, 
        [FromBody] SynchronizationActionRequest synchronizationActionRequest)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new FileOrDirectoryIsDeletedRequest(sessionId, client, synchronizationActionRequest.ActionsGroupIds,
            synchronizationActionRequest.NodeId);
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("directoryIsCreated")]
    public async Task<IActionResult> DirectoryIsCreated(string sessionId, 
        [FromBody] SynchronizationActionRequest synchronizationActionRequest)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new DirectoryIsCreatedRequest(sessionId, client, synchronizationActionRequest.ActionsGroupIds,
            synchronizationActionRequest.NodeId);
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("memberHasFinished")]
    public async Task<IActionResult> MemberHasFinished(string sessionId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new MemberHasFinishedRequest(sessionId, client);
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("abort")]
    public async Task<IActionResult> Abort(string sessionId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new RequestSynchronizationAbortRequest(sessionId, client);
        await _mediator.Send(request);
        return Ok();
    }

    [HttpPost("errors")]
    public async Task<IActionResult> SynchronizationErrors(string sessionId, 
        [FromBody] SynchronizationActionRequest synchronizationActionRequest)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new SynchronizationErrorsRequest(sessionId, client, synchronizationActionRequest.ActionsGroupIds,
            synchronizationActionRequest.NodeId);
        await _mediator.Send(request);
        return Ok();
    }
}
