using ByteSync.Api.Helpers;
using ByteSync.Common.Business.SharedFiles;
using ByteSync.ServerCommon.Commands.FileTransfers;
using ByteSync.ServerCommon.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/session/{sessionId}/file")]
public class FileTransferController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;

    public FileTransferController(IMediator mediator, IClientsRepository clientsRepository)
    {
        _mediator = mediator;
        _clientsRepository = clientsRepository;
    }
    
    [HttpPost("getUploadUrl")]
    public async Task<IActionResult> GetUploadFileUrl(string sessionId, 
        [FromBody] TransferParameters transferParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new GetUploadFileUrlRequest(sessionId, client, transferParameters);
        var url = await _mediator.Send(request);
        return Ok(url);
    }
    
    [HttpPost("getUploadStorageLocation")]
    public async Task<IActionResult> GetUploadFileStorageLocation(string sessionId, 
        [FromBody] TransferParameters transferParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new GetUploadFileStorageLocationRequest(sessionId, client, transferParameters);
        var responseObject = await _mediator.Send(request);
        return Ok(responseObject);
    }

    [HttpPost("getDownloadUrl")]
    public async Task<IActionResult> GetDownloadFileUrl(string sessionId, 
        [FromBody] TransferParameters transferParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new GetDownloadFileUrlRequest(sessionId, client, transferParameters);
        var url = await _mediator.Send(request);
        return Ok(url);
    }
    
    [HttpPost("getDownloadStorageLocation")]
    public async Task<IActionResult> GetDownloadFileStorageLocation(string sessionId, 
        [FromBody] TransferParameters transferParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new GetDownloadFileStorageLocationRequest(sessionId, client, transferParameters);
        var responseObject = await _mediator.Send(request);
        return Ok(responseObject);
    }
    
    [HttpPost("partUploaded")]
    public async Task<IActionResult> AssertFilePartIsUploaded(string sessionId, 
        [FromBody] TransferParameters transferParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new AssertFilePartIsUploadedRequest(sessionId, client, transferParameters);
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("partDownloaded")]
    public async Task<IActionResult> AssertFilePartIsDownloaded(string sessionId, 
        [FromBody] TransferParameters transferParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new AssertFilePartIsDownloadedRequest(sessionId, client, transferParameters);
        await _mediator.Send(request);
        return Ok();
    }
    
    [HttpPost("uploadFinished")]
    public async Task<IActionResult> AssertUploadIsFinished(string sessionId, 
        [FromBody] TransferParameters transferParameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new AssertUploadIsFinishedRequest(sessionId, client, transferParameters);
        await _mediator.Send(request);
        return Ok();
    }
}
