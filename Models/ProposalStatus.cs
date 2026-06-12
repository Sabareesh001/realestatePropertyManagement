using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ProposalStatus
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<LeaseProposal> LeaseProposals { get; set; } = new List<LeaseProposal>();

    /// <summary>
    /// Gets or sets the date and time when the proposal status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the proposal status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the proposal status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
