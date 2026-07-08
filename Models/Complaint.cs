using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Represents a complaint raised by a tenant against a property/lease.
/// </summary>
public partial class Complaint
{
    /// <summary>Gets or sets the unique identifier of the complaint.</summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Gets or sets the lease this complaint is associated with.</summary>
    public Guid? LeaseId { get; set; }

    /// <summary>Gets or sets the tenant who raised the complaint.</summary>
    public Guid? TenantId { get; set; }

    /// <summary>Gets or sets the property the complaint is against.</summary>
    public int? PropertyId { get; set; }

    /// <summary>Gets or sets the property owner's user id (derived at create time).</summary>
    public Guid? OwnerId { get; set; }

    /// <summary>Gets or sets the category (complaint type) identifier.</summary>
    public int? ComplaintTypeId { get; set; }

    /// <summary>Gets or sets the status identifier.</summary>
    public int? StatusId { get; set; }

    /// <summary>Gets or sets the priority identifier.</summary>
    public int? PriorityId { get; set; }

    /// <summary>Gets or sets the short subject of the complaint (5–150 chars).</summary>
    public string? Subject { get; set; }

    /// <summary>Gets or sets the detailed description (10–2000 chars).</summary>
    public string? Description { get; set; }

    /// <summary>Gets or sets an optional attachment URL.</summary>
    public string? AttachmentUrl { get; set; }

    /// <summary>Gets or sets the user who created the complaint (same as TenantId on creation).</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>Gets or sets the timestamp when the complaint was resolved.</summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>Gets or sets the user who resolved the complaint.</summary>
    public Guid? ResolvedBy { get; set; }

    /// <summary>Gets or sets the lease navigation property.</summary>
    public virtual Lease? Lease { get; set; }

    /// <summary>Gets or sets the category navigation property.</summary>
    public virtual ComplaintType? ComplaintType { get; set; }

    /// <summary>Gets or sets the priority navigation property.</summary>
    public virtual ComplaintPriority? Priority { get; set; }

    /// <summary>Gets or sets the property navigation property.</summary>
    public virtual Property? Property { get; set; }

    /// <summary>Gets or sets the user who resolved this complaint.</summary>
    public virtual User? ResolvedByNavigation { get; set; }

    /// <summary>Gets or sets the status navigation property.</summary>
    public virtual ComplaintStatus? Status { get; set; }

    /// <summary>Gets or sets the tenant navigation property.</summary>
    public virtual User? Tenant { get; set; }

    /// <summary>Gets or sets the collection of comments on this complaint.</summary>
    public virtual ICollection<ComplaintComment> Comments { get; set; } = new List<ComplaintComment>();

    /// <summary>Gets or sets the date and time when the complaint was created.</summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>Gets or sets the date and time when the complaint was last updated.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Gets or sets the date and time when the complaint was soft deleted.</summary>
    public DateTime? DeletedAt { get; set; }
}
