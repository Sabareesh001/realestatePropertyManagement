using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ProfileType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();

    /// <summary>
    /// Gets or sets the date and time when the profile type was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the profile type was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the profile type was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
