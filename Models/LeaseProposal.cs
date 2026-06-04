using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class LeaseProposal
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? TenantId { get; set; }

    public int? PropertyId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? MonthlyRent { get; set; }

    public decimal? UpfrontPayment { get; set; }

    public decimal? SecurityDeposit { get; set; }

    public int? LeaseTypeId { get; set; }

    public int? StatusId { get; set; }

    public Guid? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual LeaseType? LeaseType { get; set; }

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    public virtual Property? Property { get; set; }

    public virtual User? ReviewedByNavigation { get; set; }

    public virtual ProposalStatus? Status { get; set; }

    public virtual User? Tenant { get; set; }
}
