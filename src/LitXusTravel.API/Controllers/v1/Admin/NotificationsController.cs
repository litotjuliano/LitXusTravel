using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LitXusTravel.Application.UseCases.Notifications.GetAdminNotifications;
using LitXusTravel.Application.UseCases.Notifications.MarkNotificationsRead;

namespace LitXusTravel.API.Controllers.v1.Admin;

[ApiController]
[Route("api/v1/admin/notifications")]
[Authorize(Roles = "SuperAdmin,Admin")]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    /// <summary>Get notifications for the current admin user</summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await mediator.Send(new GetAdminNotificationsQuery(userId), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });

        var notifications = result.Value!;
        return Ok(new
        {
            data = notifications,
            unreadCount = notifications.Count(n => !n.IsRead)
        });
    }

    /// <summary>Mark a single notification as read</summary>
    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await mediator.Send(new MarkNotificationsReadCommand(userId, id), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok();
    }

    /// <summary>Mark all notifications as read for the current user</summary>
    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await mediator.Send(new MarkNotificationsReadCommand(userId, null), ct);
        if (!result.IsSuccess) return BadRequest(new { result.Errors });
        return Ok();
    }
}
