using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ProposalStatus
{
    /// <summary>
    /// Status indicating the proposal is in draft.
    /// </summary>
    public const int Draft = 1;

    /// <summary>
    /// Status indicating the proposal has been submitted by the tenant.
    /// </summary>
    public const int Submitted = 2;

    /// <summary>
    /// Status indicating the proposal has been approved.
    /// </summary>
    public const int Approved = 3;

    /// <summary>
    /// Status indicating the proposal has been rejected.
    /// </summary>
    public const int Rejected = 4;

    /// <summary>
    /// Status indicating the proposal has expired.
    /// </summary>
    public const int Expired = 5;

    /// <summary>
    /// Status indicating the proposal has been cancelled.
    /// </summary>
    public const int Cancelled = 6;

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
