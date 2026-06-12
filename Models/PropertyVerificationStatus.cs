using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Entity representing the verification status of a property.
/// </summary>
public partial class PropertyVerificationStatus
{
    /// <summary>
    /// Property is in draft mode.
    /// </summary>
    public const int Draft = 1;

    /// <summary>
    /// Property has been submitted for verification.
    /// </summary>
    public const int Submitted = 2;

    /// <summary>
    /// Property is verified by an admin.
    /// </summary>
    public const int Verified = 3;

    /// <summary>
    /// Property is rejected by an admin.
    /// </summary>
    public const int Rejected = 4;

    /// <summary>
    /// Gets or sets the unique identifier for the property verification status.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the verification status (e.g. "Draft", "Submitted").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the properties associated with this verification status.
    /// </summary>
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

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
