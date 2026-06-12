using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ComplaintStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    /// <summary>
    /// Gets or sets the date and time when the complaint status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
