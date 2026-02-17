using System.Net;
using ByteSync.Common.Business.Auth;
using ByteSync.ServerCommon.Commands.Authentication;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginData loginData)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var authResult = await _mediator.Send(new AuthenticateRequest(loginData, ip));

        if (authResult.IsSuccess)
        {
            return Ok(authResult);
        }
        
        return Unauthorized(authResult);
    }
    
    [HttpPost("refreshTokens")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensData refreshTokensData)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        var authResult = await _mediator.Send(new RefreshTokensRequest(refreshTokensData, ip));
        
        if (authResult.IsSuccess)
        {
            return Ok(authResult);
        }
        
        return Unauthorized(authResult);
    }

    [AllowAnonymous]
    [HttpPost("negotiate")]
    public IActionResult Negotiate()
    {
        var hubUrl = $"{Request.Scheme}://{Request.Host}/hubs/bytesync";
        
        return Ok(new
        {
            Url = hubUrl,
            AccessToken = string.Empty
        });
    }
}
