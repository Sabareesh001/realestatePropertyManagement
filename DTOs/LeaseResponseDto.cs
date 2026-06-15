using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a lease in API responses.
/// </summary>
public class LeaseResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the lease.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the tenant user's identifier.
    /// </summary>
    public Guid? TenantId { get; set; }

    /// <summary>
    /// Gets or sets the property identifier.
    /// </summary>
    public int? PropertyId { get; set; }

    /// <summary>
    /// Gets or sets the associated proposal (rent request) identifier.
    /// </summary>
    public Guid ProposalId { get; set; }

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
    /// Gets or sets the status identifier.
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the name of the status.
    /// </summary>
    public string? StatusName { get; set; }

    /// <summary>
    /// Gets or sets the URL of the agreement template document.
    /// </summary>
    public string? AgreementDocumentUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL of the tenant-signed agreement document.
    /// </summary>
    public string? SignedAgreementDocumentUrl { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last updated timestamp.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
