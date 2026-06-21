using System;
using System.Collections.Generic;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for recording a payment against lease charges.
/// </summary>
public class RecordPaymentDto
{
    /// <summary>
    /// Gets or sets the list of charge allocations specifying how the payment is distributed across charges.
    /// </summary>
    public List<ChargeAllocationDto> ChargeAllocations { get; set; } = new();

    /// <summary>
    /// Gets or sets the payment method identifier.
    /// </summary>
    public int PaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the unique transaction reference for the payment.
    /// </summary>
    public string TransactionRef { get; set; } = null!;

    /// <summary>
    /// Gets or sets the currency identifier. Defaults to INR (1) if not specified.
    /// </summary>
    public int CurrencyId { get; set; } = 1;
}

/// <summary>
/// Data Transfer Object representing a single charge allocation within a payment.
/// </summary>
public class ChargeAllocationDto
{
    /// <summary>
    /// Gets or sets the charge identifier to apply payment to.
    /// </summary>
    public Guid ChargeId { get; set; }

    /// <summary>
    /// Gets or sets the amount to apply towards this charge.
    /// </summary>
    public decimal Amount { get; set; }
}
