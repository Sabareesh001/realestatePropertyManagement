using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class PaymentStatus
{
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
