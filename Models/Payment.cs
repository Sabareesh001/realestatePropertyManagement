using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

public partial class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string? TransactionRef { get; set; }

    public decimal? Amount { get; set; }

    public int? CurrencyId { get; set; }

    public Guid? PaidBy { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? StatusId { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ChargePayment> ChargePayments { get; set; } = new List<ChargePayment>();

    public virtual Currency? Currency { get; set; }

    public virtual User? PaidByNavigation { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual PaymentStatus? Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the payment was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the payment was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
