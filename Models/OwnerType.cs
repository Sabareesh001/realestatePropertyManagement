using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class OwnerType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<OwnerProfile> OwnerProfiles { get; set; } = new List<OwnerProfile>();

    /// <summary>
    /// Gets or sets the date and time when the owner type was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the owner type was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the owner type was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
