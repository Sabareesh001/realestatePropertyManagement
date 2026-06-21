using System;
using System.Collections.Generic;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a payment in API responses.
/// </summary>
public class PaymentResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the payment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the total payment amount.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Gets or sets the unique transaction reference.
    /// </summary>
    public string? TransactionRef { get; set; }

    /// <summary>
    /// Gets or sets the payment method identifier.
    /// </summary>
    public int? PaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the payment method name.
    /// </summary>
    public string? PaymentMethodName { get; set; }

    /// <summary>
    /// Gets or sets the payment status identifier.
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the payment status name.
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who made the payment.
    /// </summary>
    public Guid? PaidBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the payment was made.
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// Gets or sets the currency identifier.
    /// </summary>
    public int? CurrencyId { get; set; }

    /// <summary>
    /// Gets or sets the list of charge allocations showing how the payment was distributed.
    /// </summary>
    public List<ChargeAllocationResponseDto> ChargeAllocations { get; set; } = new();

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Data Transfer Object representing a charge allocation within a payment response.
/// </summary>
public class ChargeAllocationResponseDto
{
    /// <summary>
    /// Gets or sets the charge identifier.
    /// </summary>
    public Guid ChargeId { get; set; }

    /// <summary>
    /// Gets or sets the amount applied to this charge.
    /// </summary>
    public decimal? AmountApplied { get; set; }
}
