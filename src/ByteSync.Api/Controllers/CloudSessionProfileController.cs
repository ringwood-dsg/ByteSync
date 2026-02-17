using ByteSync.Api.Helpers;
using ByteSync.Common.Business.Lobbies.Connections;
using ByteSync.Common.Business.Profiles;
using ByteSync.ServerCommon.Interfaces.Repositories;
using ByteSync.ServerCommon.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/cloudSessionProfile")]
public class CloudSessionProfileController : ControllerBase
{
    private readonly ICloudSessionProfileService _cloudSessionProfileService;
    private readonly IClientsRepository _clientsRepository;

    public CloudSessionProfileController(ICloudSessionProfileService cloudSessionProfileService, 
        IClientsRepository clientsRepository)
    {
        _cloudSessionProfileService = cloudSessionProfileService;
        _clientsRepository = clientsRepository;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCloudSessionProfile([FromBody] string sessionId)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var result = await _cloudSessionProfileService.CreateCloudSessionProfile(sessionId, client);
        return Ok(result);
    }
    
    [HttpPost("{cloudSessionProfileId}/get")]
    public async Task<IActionResult> GetCloudSessionProfileData(string cloudSessionProfileId, 
        [FromBody] GetCloudSessionProfileDataParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var result = await _cloudSessionProfileService.GetCloudSessionProfileData(parameters, client);
        return Ok(result);
    }
    
    [HttpPost("{cloudSessionProfileId}/getProfileDetailsPassword")]
    public async Task<IActionResult> GetProfileDetailsPassword(string cloudSessionProfileId, 
        [FromBody] GetProfileDetailsPasswordParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var result = await _cloudSessionProfileService.GetProfileDetailsPassword(parameters, client);
        return Ok(result);
    }
    
    [HttpPost("{cloudSessionProfileId}/delete")]
    public async Task<IActionResult> DeleteCloudSessionProfile(string cloudSessionProfileId, 
        [FromBody] DeleteCloudSessionProfileParameters parameters)
    {
        var client = await ControllerHelper.GetClientFromContext(HttpContext, _clientsRepository);
        var result = await _cloudSessionProfileService.DeleteCloudSessionProfile(parameters, client);
        return Ok(result);
    }
}
