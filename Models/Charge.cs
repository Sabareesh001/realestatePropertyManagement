using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Charge
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? Desc { get; set; }

    public int? ChargeTypeId { get; set; }

    public decimal? Amount { get; set; }

    public int? StatusId { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChargePayment> ChargePayments { get; set; } = new List<ChargePayment>();

    public virtual ChargeType? ChargeType { get; set; }

    public virtual ChargeStatus? Status { get; set; }

    public virtual ICollection<Lease> Leases { get; set; } = new List<Lease>();

    public virtual ICollection<Lease> Users { get; set; } = new List<Lease>();
}
