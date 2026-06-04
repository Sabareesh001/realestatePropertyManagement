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
}
