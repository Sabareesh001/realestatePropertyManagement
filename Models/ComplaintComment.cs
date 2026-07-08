using System;

namespace propertyManagement.Models;

/// <summary>
/// Represents a threaded comment on a complaint, posted by any participant or admin.
/// </summary>
public class ComplaintComment
{
    /// <summary>Gets or sets the unique identifier of the comment.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the complaint this comment belongs to.</summary>
    public Guid ComplaintId { get; set; }

    /// <summary>Gets or sets the user who posted this comment.</summary>
    public Guid AuthorId { get; set; }

    /// <summary>Gets or sets the message body (1–2000 chars).</summary>
    public string Message { get; set; } = null!;

    /// <summary>Gets or sets the complaint navigation property.</summary>
    public virtual Complaint Complaint { get; set; } = null!;

    /// <summary>Gets or sets the author navigation property.</summary>
    public virtual User Author { get; set; } = null!;

    /// <summary>Gets or sets the timestamp when the comment was posted.</summary>
    public DateTime? CreatedAt { get; set; }
}
