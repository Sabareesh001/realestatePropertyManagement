using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a lease proposal in API responses.
/// </summary>
public class LeaseProposalResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the lease proposal.
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
    /// Gets or sets the lease start date.
    /// </summary>
    public DateOnly? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the lease end date.
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the proposed monthly rent.
    /// </summary>
    public decimal? MonthlyRent { get; set; }

    /// <summary>
    /// Gets or sets the proposed upfront payment.
    /// </summary>
    public decimal? UpfrontPayment { get; set; }

    /// <summary>
    /// Gets or sets the proposed security deposit.
    /// </summary>
    public decimal? SecurityDeposit { get; set; }

    /// <summary>
    /// Gets or sets the status identifier.
    /// </summary>
    public int? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the admin reviewer's identifier.
    /// </summary>
    public Guid? ReviewedBy { get; set; }

    /// <summary>
    /// Gets or sets the review timestamp.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}
