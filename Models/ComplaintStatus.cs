using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ComplaintStatus
{
    /// <summary>Status indicating the complaint is open and awaiting action.</summary>
    public const int Open = 1;

    /// <summary>Status indicating the complaint is being actively worked on.</summary>
    public const int InProgress = 2;

    /// <summary>Status indicating the complaint has been resolved by the owner/admin.</summary>
    public const int Resolved = 3;

    /// <summary>Status indicating the complaint has been closed by the tenant.</summary>
    public const int Closed = 4;

    /// <summary>Status indicating the complaint has been cancelled.</summary>
    public const int Cancelled = 5;

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
