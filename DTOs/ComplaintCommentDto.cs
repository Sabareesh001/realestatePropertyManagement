using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a single comment on a complaint thread.
/// </summary>
public class ComplaintCommentDto
{
    /// <summary>Gets or sets the unique identifier of the comment.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the complaint this comment belongs to.</summary>
    public Guid ComplaintId { get; set; }

    /// <summary>Gets or sets the author's user identifier.</summary>
    public Guid AuthorId { get; set; }

    /// <summary>Gets or sets the author's full name.</summary>
    public string? AuthorName { get; set; }

    /// <summary>Gets or sets the computed role of the author: Admin, Owner, or Tenant.</summary>
    public string? AuthorRole { get; set; }

    /// <summary>Gets or sets the message body.</summary>
    public string? Message { get; set; }

    /// <summary>Gets or sets when the comment was posted.</summary>
    public DateTime? CreatedAt { get; set; }
}
