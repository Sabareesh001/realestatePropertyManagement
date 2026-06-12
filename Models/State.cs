using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class State
{
    public int Id { get; set; }

    public int? CountryId { get; set; }

    public string? Name { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    /// <summary>
    /// Gets or sets the date and time when the state was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the state was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the state was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
