using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service for persisting and pushing real-time notifications over SignalR.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Persists one notification per recipient and pushes it in real time to any of them
    /// who currently have an active SignalR connection.
    /// </summary>
    /// <param name="recipientIds">The identifiers of the users to notify.</param>
    /// <param name="notificationTypeId">The notification type (see <see cref="propertyManagement.Models.NotificationType"/>).</param>
    /// <param name="title">The notification title.</param>
    /// <param name="message">The notification message body.</param>
    /// <param name="relatedEntityType">Optional type of the entity this notification relates to.</param>
    /// <param name="relatedEntityId">Optional identifier of the related entity.</param>
    Task NotifyAsync(
        IReadOnlyCollection<Guid> recipientIds,
        int notificationTypeId,
        string title,
        string message,
        string? relatedEntityType = null,
        Guid? relatedEntityId = null);

    /// <summary>
    /// Retrieves a page of notifications addressed to the given user, newest first.
    /// </summary>
    /// <param name="userId">The recipient's user identifier.</param>
    /// <param name="pagination">The pagination parameters.</param>
    Task<PagedResultDto<NotificationResponseDto>> GetMyNotificationsAsync(Guid userId, PaginationParams pagination);

    /// <summary>
    /// Marks a notification as read on behalf of its recipient.
    /// </summary>
    /// <param name="userId">The identifier of the user marking the notification as read.</param>
    /// <param name="notificationId">The notification identifier.</param>
    /// <exception cref="KeyNotFoundException">Thrown when the notification is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not the recipient of the notification.</exception>
    Task MarkAsReadAsync(Guid userId, Guid notificationId);
}
