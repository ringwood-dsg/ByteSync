using ByteSync.Api.Helpers;
using ByteSync.Common.Business.Inventories;
using ByteSync.Common.Business.Sessions;
using ByteSync.Common.Business.Sessions.Cloud;
using ByteSync.ServerCommon.Commands.Inventories;
using ByteSync.ServerCommon.Interfaces.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/session/{sessionId}/inventory")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClientsRepository _clientsRepository;

    public InventoryController(IMediator mediator, IClientsRepository clientsRepository)
    {
        _mediator = mediator;
        _clientsRepository = clientsRepository;
    }
    
    [HttpPost("start")]
    public async Task<IActionResult> Start(string sessionId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new StartInventoryRequest(sessionId, client);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    
    [HttpPost("{clientInstanceId}/dataNode/{dataNodeId}/dataSource")]
    public async Task<IActionResult> AddDataSource(string sessionId, string clientInstanceId, string dataNodeId, 
        [FromBody] EncryptedDataSource encryptedDataSource)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new AddDataSourceRequest(sessionId, client, clientInstanceId, dataNodeId, encryptedDataSource);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    
    [HttpDelete("{clientInstanceId}/dataNode/{dataNodeId}/dataSource")]
    public async Task<IActionResult> RemoveDataSource(string sessionId, string clientInstanceId, string dataNodeId, 
        [FromBody] EncryptedDataSource encryptedDataSource)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new RemoveDataSourceRequest(sessionId, client, clientInstanceId, dataNodeId, encryptedDataSource);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    
    [HttpGet("{clientInstanceId}/dataNode/{dataNodeId}/dataSource")]
    public async Task<IActionResult> GetDataSources(string sessionId, string clientInstanceId, string dataNodeId)
    {
        var request = new GetDataSourcesRequest(sessionId, clientInstanceId, dataNodeId);
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPost("{clientInstanceId}/dataNode")]
    public async Task<IActionResult> AddDataNode(string sessionId, string clientInstanceId, 
        [FromBody] EncryptedDataNode encryptedDataNode)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new AddDataNodeRequest(sessionId, client, clientInstanceId, encryptedDataNode);
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{clientInstanceId}/dataNode")]
    public async Task<IActionResult> RemoveDataNode(string sessionId, string clientInstanceId, 
        [FromBody] EncryptedDataNode encryptedDataNode)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var request = new RemoveDataNodeRequest(sessionId, client, clientInstanceId, encryptedDataNode);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
    
    [HttpGet("{clientInstanceId}/dataNode")]
    public async Task<IActionResult> GetDataNodes(string sessionId, string clientInstanceId)
    {
        var request = new GetDataNodesRequest(sessionId, clientInstanceId);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}
