using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ComplaintType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    /// <summary>
    /// Gets or sets the date and time when the complaint type was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint type was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the complaint type was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
