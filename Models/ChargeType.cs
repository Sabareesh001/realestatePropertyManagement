using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ChargeType
{
    /// <summary>
    /// Charge type for monthly rent.
    /// </summary>
    public const int MonthlyRent = 1;

    /// <summary>
    /// Charge type for security deposit.
    /// </summary>
    public const int SecurityDeposit = 2;

    /// <summary>
    /// Charge type for upfront payment.
    /// </summary>
    public const int UpfrontPayment = 3;

    /// <summary>
    /// Charge type for maintenance fees.
    /// </summary>
    public const int Maintenance = 4;

    /// <summary>
    /// Charge type for penalty charges.
    /// </summary>
    public const int Penalty = 5;

    /// <summary>
    /// Charge type for miscellaneous/other charges.
    /// </summary>
    public const int Other = 6;

    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    /// <summary>
    /// Gets or sets the date and time when the charge type was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the charge type was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the charge type was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
