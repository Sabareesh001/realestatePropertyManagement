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
}
