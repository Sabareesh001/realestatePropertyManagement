using System;
using System.Collections.Generic;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a complaint in API responses.
/// List endpoints set Comments to an empty list; detail endpoint includes full thread.
/// </summary>
public class ComplaintResponseDto
{
    /// <summary>Gets or sets the unique identifier of the complaint.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the lease identifier.</summary>
    public Guid? LeaseId { get; set; }

    /// <summary>Gets or sets the property identifier.</summary>
    public int? PropertyId { get; set; }

    /// <summary>Gets or sets the property title.</summary>
    public string? PropertyTitle { get; set; }

    /// <summary>Gets or sets the tenant's user identifier.</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the tenant's full name.</summary>
    public string? TenantName { get; set; }

    /// <summary>Gets or sets the property owner's user identifier.</summary>
    public Guid? OwnerId { get; set; }

    /// <summary>Gets or sets the category (complaint type) identifier.</summary>
    public int? CategoryId { get; set; }

    /// <summary>Gets or sets the category display name.</summary>
    public string? CategoryName { get; set; }

    /// <summary>Gets or sets the priority identifier.</summary>
    public int? PriorityId { get; set; }

    /// <summary>Gets or sets the priority display name.</summary>
    public string? PriorityName { get; set; }

    /// <summary>Gets or sets the status identifier.</summary>
    public int? StatusId { get; set; }

    /// <summary>Gets or sets the status display name.</summary>
    public string? StatusName { get; set; }

    /// <summary>Gets or sets the short subject of the complaint.</summary>
    public string? Subject { get; set; }

    /// <summary>Gets or sets the detailed description.</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets an optional attachment URL.</summary>
    public string? AttachmentUrl { get; set; }

    /// <summary>Gets or sets the identifier of the user who created the complaint.</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>Gets or sets when the complaint was resolved (null if not yet resolved).</summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>Gets or sets when the complaint was created.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>Gets or sets when the complaint was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Gets or sets the total number of comments on this complaint.</summary>
    public int CommentCount { get; set; }

    /// <summary>Gets or sets the comment thread. Empty list on list endpoints; full list on detail endpoint.</summary>
    public List<ComplaintCommentDto> Comments { get; set; } = new();
}
