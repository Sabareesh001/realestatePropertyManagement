using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class OwnerProfile
{
    public Guid UserId { get; set; }

    public int? OwnerTypeId { get; set; }

    public string? OrganizationName { get; set; }

    public string? Gstin { get; set; }

    public virtual OwnerType? OwnerType { get; set; }

    public virtual UserProfile User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the owner profile was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the owner profile was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the owner profile was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
