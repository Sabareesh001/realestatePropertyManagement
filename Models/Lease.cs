using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Lease
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? TenantId { get; set; }

    public int? PropertyId { get; set; }

    public Guid? ProposalId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? MonthlyRent { get; set; }

    public decimal? UpfrontPayment { get; set; }

    public decimal? SecurityDeposit { get; set; }

    public int? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the agreement template document.
    /// </summary>
    public Guid? AgreementDocumentId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the tenant-signed agreement document.
    /// </summary>
    public Guid? SignedAgreementDocumentId { get; set; }

    /// <summary>
    /// Gets or sets the agreement template document associated with the lease.
    /// </summary>
    public virtual Document? AgreementDocument { get; set; }

    /// <summary>
    /// Gets or sets the tenant-signed agreement document associated with the lease.
    /// </summary>
    public virtual Document? SignedAgreementDocument { get; set; }

    public virtual Property? PropertyNavigation { get; set; }


    public virtual LeaseProposal? Proposal { get; set; }

    public virtual LeaseStatus? Status { get; set; }

    public virtual User? Tenant { get; set; }

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public virtual ICollection<Charge> ChargesNavigation { get; set; } = new List<Charge>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    /// <summary>
    /// Gets or sets the date and time when the lease was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the lease was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the lease was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
