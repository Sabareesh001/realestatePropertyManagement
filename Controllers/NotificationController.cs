using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for retrieving and managing the current user's notifications.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotificationController : BaseApiController
{
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Initializes a new instance of <see cref="NotificationController"/>.
    /// </summary>
    /// <param name="notificationService">The notification service.</param>
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    /// <summary>
    /// Retrieves a page of notifications addressed to the currently authenticated user, newest first.
    /// </summary>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of notifications.</returns>
    /// <response code="200">Notifications retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<NotificationResponseDto>>> GetMyNotifications([FromQuery] PaginationParams pagination)
    {
        var userId = GetCurrentUserId();
        var result = await _notificationService.GetMyNotificationsAsync(userId, pagination);
        return Ok(result);
    }

    /// <summary>
    /// Marks a notification as read on behalf of the currently authenticated user.
    /// </summary>
    /// <param name="id">The unique identifier of the notification.</param>
    /// <response code="204">Notification marked as read.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not the recipient of the notification.</response>
    /// <response code="404">If the notification is not found.</response>
    [Authorize]
    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        var userId = GetCurrentUserId();
        await _notificationService.MarkAsReadAsync(userId, id);
        return NoContent();
    }
}
