using System;

namespace propertyManagement.Models;

/// <summary>
/// Represents a persisted notification delivered (or pending delivery) to a single recipient.
/// </summary>
public partial class Notification
{
    /// <summary>Gets or sets the unique identifier of the notification.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the identifier of the user this notification is for.</summary>
    public Guid RecipientId { get; set; }

    /// <summary>Gets or sets the notification type identifier (see <see cref="NotificationType"/>).</summary>
    public int TypeId { get; set; }

    /// <summary>Gets or sets the short notification title.</summary>
    public string Title { get; set; } = null!;

    /// <summary>Gets or sets the notification message body.</summary>
    public string Message { get; set; } = null!;

    /// <summary>Gets or sets the type of entity this notification relates to (e.g. "LeaseProposal", "Lease").</summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>Gets or sets the identifier of the related entity.</summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>Gets or sets whether the recipient has read this notification.</summary>
    public bool IsRead { get; set; }

    /// <summary>Gets or sets the timestamp when the recipient read this notification.</summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>Gets or sets the recipient navigation property.</summary>
    public virtual User? Recipient { get; set; }

    /// <summary>Gets or sets the date and time when the notification was created.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>Gets or sets the date and time when the notification was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Gets or sets the date and time when the notification was soft deleted.</summary>
    public DateTime? DeletedAt { get; set; }
}
