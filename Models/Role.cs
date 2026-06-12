using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Entity representing a security role in the property management system.
/// </summary>
public partial class Role
{
    /// <summary>
    /// The unique identifier for the Tenant role.
    /// </summary>
    public const int Tenant = 1;

    /// <summary>
    /// The unique identifier for the Owner role.
    /// </summary>
    public const int Owner = 2;

    /// <summary>
    /// The unique identifier for the Admin role.
    /// </summary>
    public const int Admin = 3;

    /// <summary>
    /// Gets or sets the unique identifier of the role.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the role.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the role was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the role was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the role was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the user-role associations for this role.
    /// </summary>
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
