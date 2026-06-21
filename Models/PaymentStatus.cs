using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class PaymentStatus
{
    /// <summary>
    /// Status indicating the payment is pending.
    /// </summary>
    public const int Pending = 1;

    /// <summary>
    /// Status indicating the payment has been completed.
    /// </summary>
    public const int Completed = 2;

    /// <summary>
    /// Status indicating the payment has failed.
    /// </summary>
    public const int Failed = 3;

    /// <summary>
    /// Status indicating the payment has been refunded.
    /// </summary>
    public const int Refunded = 4;

    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    /// <summary>
    /// Gets or sets the date and time when the payment status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the payment status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the payment status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
