using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class OwnerType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<OwnerProfile> OwnerProfiles { get; set; } = new List<OwnerProfile>();
}
