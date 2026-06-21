using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for creating a new charge on a lease.
/// </summary>
public class CreateChargeDto
{
    /// <summary>
    /// Gets or sets the charge type identifier.
    /// </summary>
    public int ChargeTypeId { get; set; }

    /// <summary>
    /// Gets or sets the charge amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the description/notes for the charge.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the due date of the charge.
    /// </summary>
    public DateTime DueDate { get; set; }
}
