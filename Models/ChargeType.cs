using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ChargeType
{
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
