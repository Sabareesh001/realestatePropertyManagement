using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Complaint
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? TenantId { get; set; }

    public int? PropertyId { get; set; }

    public int? ComplaintTypeId { get; set; }

    public int? StatusId { get; set; }

    public int? PriorityId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public Guid? ResolvedBy { get; set; }

    public virtual ComplaintType? ComplaintType { get; set; }

    public virtual ComplaintPriority? Priority { get; set; }

    public virtual Property? Property { get; set; }

    public virtual User? ResolvedByNavigation { get; set; }

    public virtual ComplaintStatus? Status { get; set; }

    public virtual User? Tenant { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
