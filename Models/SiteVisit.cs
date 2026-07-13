using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class SiteVisit
{
    public Guid Id { get; set; }

    public int PropertyId { get; set; }

    public Guid TenantId { get; set; }

    public Guid OwnerId { get; set; }

    public DateTime VisitDate { get; set; }

    public int StatusId { get; set; }

    public string? Remarks { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User Tenant { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;

    public virtual SiteVisitStatus Status { get; set; } = null!;
}
