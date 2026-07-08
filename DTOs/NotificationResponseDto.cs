using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a notification in API responses and real-time pushes.
/// </summary>
public class NotificationResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the notification.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the notification type identifier (see <see cref="propertyManagement.Models.NotificationType"/>).
    /// </summary>
    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets the short notification title.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the notification message body.
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Gets or sets the type of entity this notification relates to.
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the related entity.
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// Gets or sets whether the recipient has read this notification.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the notification was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}
