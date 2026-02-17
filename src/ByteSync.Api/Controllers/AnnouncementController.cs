using ByteSync.ServerCommon.Commands.Announcements;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ByteSync.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnnouncementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAnnouncements()
    {
        var announcements = await _mediator.Send(new GetActiveAnnouncementsRequest());
        return Ok(announcements);
    }
}
