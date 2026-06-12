using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class TenantProfile
{
    public Guid UserId { get; set; }

    public decimal? MonthlyIncome { get; set; }

    public string? Occupation { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactNumber { get; set; }

    public virtual UserProfile User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the tenant profile was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the tenant profile was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the tenant profile was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
