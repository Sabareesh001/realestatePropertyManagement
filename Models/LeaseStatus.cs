using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class LeaseStatus
{
    /// <summary>
    /// Status indicating the lease is a draft.
    /// </summary>
    public const int Draft = 1;

    /// <summary>
    /// Status indicating the lease has been submitted by the owner.
    /// </summary>
    public const int Submitted = 2;

    /// <summary>
    /// Status indicating the lease is pending the tenant's signature.
    /// </summary>
    public const int PendingSignature = 3;

    /// <summary>
    /// Status indicating the tenant has signed the lease.
    /// </summary>
    public const int TenantSigned = 4;

    /// <summary>
    /// Status indicating the lease is active.
    /// </summary>
    public const int Active = 5;

    /// <summary>
    /// Status indicating the lease has been rejected by the admin.
    /// </summary>
    public const int Rejected = 6;

    /// <summary>
    /// Status indicating the lease has been terminated.
    /// </summary>
    public const int Terminated = 7;

    /// <summary>
    /// Status indicating the lease has expired.
    /// </summary>
    public const int Expired = 8;

    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    /// <summary>
    /// Gets or sets the date and time when the lease status was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the lease status was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the lease status was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
