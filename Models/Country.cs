using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Country
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();

    /// <summary>
    /// Gets or sets the date and time when the country was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the country was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the country was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
