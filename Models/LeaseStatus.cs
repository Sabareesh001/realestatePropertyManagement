using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class LeaseStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();
}
