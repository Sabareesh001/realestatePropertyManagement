using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for updating an existing lease draft.
/// </summary>
public class UpdateLeaseDto
{
    /// <summary>
    /// Gets or sets the lease start date.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the lease end date.
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the monthly rent.
    /// </summary>
    public decimal? MonthlyRent { get; set; }

    /// <summary>
    /// Gets or sets the upfront payment.
    /// </summary>
    public decimal? UpfrontPayment { get; set; }

    /// <summary>
    /// Gets or sets the security deposit.
    /// </summary>
    public decimal? SecurityDeposit { get; set; }

    /// <summary>
    /// Gets or sets the status of the lease (e.g., set to Submitted = 2).
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the agreement document.
    /// </summary>
    public string? AgreementDocumentUrl { get; set; }
}
