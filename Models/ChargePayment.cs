using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class ChargePayment
{
    public Guid ChargeId { get; set; }

    public Guid PaymentId { get; set; }

    public decimal? AmountApplied { get; set; }

    public virtual Charge Charge { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;
}
