using System;
using System.Collections.Generic;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a charge in API responses.
/// </summary>
public class ChargeResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the charge.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the charge type identifier.
    /// </summary>
    public int? ChargeTypeId { get; set; }

    /// <summary>
    /// Gets or sets the charge type name.
    /// </summary>
    public string? ChargeTypeName { get; set; }

    /// <summary>
    /// Gets or sets the charge amount.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Gets or sets the description/notes for the charge.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the due date of the charge.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the status identifier of the charge.
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the status name of the charge.
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// Gets or sets the total amount already paid towards this charge.
    /// </summary>
    public decimal AmountPaid { get; set; }

    /// <summary>
    /// Gets or sets the remaining balance due on this charge.
    /// </summary>
    public decimal BalanceDue { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last updated timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
