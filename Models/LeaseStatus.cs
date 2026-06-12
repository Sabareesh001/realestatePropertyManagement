using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class LeaseStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    /// <summary>
    /// Gets or sets the date and time when the lease status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the lease status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the lease status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
