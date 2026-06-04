using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class PropertyStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
