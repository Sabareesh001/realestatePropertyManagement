using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Entity representing the verification status of a user.
/// </summary>
public partial class UserVerificationStatus
{
    /// <summary>
    /// User is not verified.
    /// </summary>
    public const int Unverified = 1;

    /// <summary>
    /// User verification is pending.
    /// </summary>
    public const int Pending = 2;

    /// <summary>
    /// User is verified.
    /// </summary>
    public const int Verified = 3;

    /// <summary>
    /// User verification request has been rejected.
    /// </summary>
    public const int Rejected = 4;

    /// <summary>
    /// Gets or sets the unique identifier of the user verification status.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the verification status (e.g., "Unverified", "Pending").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the users associated with this verification status.
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Gets or sets the date and time when the verification status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the verification status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the verification status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
