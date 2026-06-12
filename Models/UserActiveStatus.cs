using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Entity representing the active status of a user.
/// </summary>
public partial class UserActiveStatus
{
    /// <summary>
    /// User account is active.
    /// </summary>
    public const int Active = 1;

    /// <summary>
    /// User account is inactive.
    /// </summary>
    public const int Inactive = 2;

    /// <summary>
    /// User account is suspended.
    /// </summary>
    public const int Suspended = 3;

    /// <summary>
    /// Gets or sets the unique identifier of the user active status.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the active status (e.g., "Active", "Inactive").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the users associated with this active status.
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Gets or sets the date and time when the active status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the active status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the active status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
