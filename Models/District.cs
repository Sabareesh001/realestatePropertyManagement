using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class District
{
    public int Id { get; set; }

    public int? StateId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual State? State { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the district was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the district was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the district was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
