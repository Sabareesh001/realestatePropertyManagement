using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ChargeStatus
{
    /// <summary>
    /// Status indicating the charge is pending payment.
    /// </summary>
    public const int Pending = 1;

    /// <summary>
    /// Status indicating the charge has been partially paid.
    /// </summary>
    public const int PartiallyPaid = 2;

    /// <summary>
    /// Status indicating the charge has been fully paid.
    /// </summary>
    public const int Paid = 3;

    /// <summary>
    /// Status indicating the charge is overdue.
    /// </summary>
    public const int Overdue = 4;

    /// <summary>
    /// Status indicating the charge has been cancelled.
    /// </summary>
    public const int Cancelled = 5;

    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    /// <summary>
    /// Gets or sets the date and time when the charge status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the charge status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the charge status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
