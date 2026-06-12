using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Entity representing the availability status of a property.
/// </summary>
public partial class PropertyAvailabilityStatus
{
    /// <summary>
    /// Property is available for rent/lease.
    /// </summary>
    public const int Available = 1;

    /// <summary>
    /// Property is currently occupied.
    /// </summary>
    public const int Occupied = 2;

    /// <summary>
    /// Property is currently unavailable.
    /// </summary>
    public const int Unavailable = 3;

    /// <summary>
    /// Gets or sets the unique identifier for the property availability status.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the availability status (e.g. "Available", "Occupied").
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the properties associated with this availability status.
    /// </summary>
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    /// <summary>
    /// Gets or sets the date and time when the availability status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the availability status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the availability status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
