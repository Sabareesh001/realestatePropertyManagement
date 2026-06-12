using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class City
{
    public int Id { get; set; }

    public int? DistrictId { get; set; }

    public string? Name { get; set; }

    public virtual District? District { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    /// <summary>
    /// Gets or sets the date and time when the city was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the city was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the city was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
