using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for creating a new complaint.
/// </summary>
public class CreateComplaintDto
{
    /// <summary>Gets or sets the lease the complaint is raised against.</summary>
    public Guid LeaseId { get; set; }

    /// <summary>Gets or sets the category identifier (1–8).</summary>
    public int CategoryId { get; set; }

    /// <summary>Gets or sets the priority identifier (1–4).</summary>
    public int PriorityId { get; set; }

    /// <summary>Gets or sets the short subject of the complaint (5–150 chars).</summary>
    public string Subject { get; set; } = null!;

    /// <summary>Gets or sets the detailed description (10–2000 chars).</summary>
    public string Description { get; set; } = null!;

    /// <summary>Gets or sets an optional attachment URL.</summary>
    public string? AttachmentUrl { get; set; }
}
