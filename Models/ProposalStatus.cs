using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ProposalStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<LeaseProposal> LeaseProposals { get; set; } = new List<LeaseProposal>();
}
