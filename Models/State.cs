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
}
