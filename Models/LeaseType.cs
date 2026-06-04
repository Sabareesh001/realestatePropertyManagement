using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class LeaseType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<LeaseProposal> LeaseProposals { get; set; } = new List<LeaseProposal>();

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();
}
