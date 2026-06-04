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

    public int? LeaseTypeId { get; set; }

    public int? StatusId { get; set; }

    public virtual LeaseType? LeaseType { get; set; }

    public virtual Property? PropertyNavigation { get; set; }

    public virtual LeaseProposal? Proposal { get; set; }

    public virtual LeaseStatus? Status { get; set; }

    public virtual User? Tenant { get; set; }

    public virtual ICollection<Charge> Charges { get; set; } = new List<Charge>();

    public virtual ICollection<Charge> ChargesNavigation { get; set; } = new List<Charge>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
