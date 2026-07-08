using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a charge enriched with platform-wide context for the admin dashboard.
/// Extends <see cref="ChargeResponseDto"/> with lease, property, owner and tenant details.
/// </summary>
public class AdminChargeDto : ChargeResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the lease the charge belongs to.
    /// </summary>
    public Guid LeaseId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the property associated with the lease, if any.
    /// </summary>
    public int? PropertyId { get; set; }

    /// <summary>
    /// Gets or sets the title of the property associated with the lease, if available.
    /// </summary>
    public string? PropertyTitle { get; set; }

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
    /// Gets or sets the full name of the tenant on the lease, if available.
    /// </summary>
    public string? TenantName { get; set; }

    /// <summary>
    /// Gets or sets the email address of the tenant on the lease, if available.
    /// </summary>
    public string? TenantEmail { get; set; }
}
