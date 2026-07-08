using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a payment enriched with platform-wide context for the admin dashboard.
/// Extends <see cref="PaymentResponseDto"/> with lease, property, owner and tenant details plus the platform fee.
/// </summary>
public class AdminPaymentDto : PaymentResponseDto
{
    /// <summary>
    /// Gets or sets the Stripe fee earned by the company for this payment.
    /// Null for manually recorded (non-Stripe) payments.
    /// </summary>
    public decimal? PlatformFee { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the lease the payment was applied to.
    /// </summary>
    public Guid LeaseId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the property associated with the lease, if any.
    /// </summary>
    public int? PropertyId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the property owner, if any.
    /// </summary>
    public Guid? OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the full name of the property owner, if available.
    /// </summary>
    public string? OwnerName { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tenant on the lease, if any.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the email address of the tenant on the lease, if available.
    /// </summary>
    public string? TenantEmail { get; set; }
}
