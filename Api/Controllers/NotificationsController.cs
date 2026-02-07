namespace Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Notifications.Queries;
using Application.Features.Notifications.Commands;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] int take = 50)
    {
        var query = new GetMyNotificationsQuery(take);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(new { message = result.Error });
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var query = new GetUnreadNotificationCountQuery();
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
            return Ok(new { count = result.Value });

        return BadRequest(new { message = result.Error });
    }

    [HttpPost("{id:long}/read")]
    public async Task<IActionResult> MarkRead(long id)
    {
        var command = new MarkNotificationReadCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "Notification marked as read" });

        return BadRequest(new { message = result.Error });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var command = new MarkAllNotificationsReadCommand();
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { message = "All notifications marked as read" });

        return BadRequest(new { message = result.Error });
    }
}
