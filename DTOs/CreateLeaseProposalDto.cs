using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for creating a new lease proposal.
/// </summary>
public class CreateLeaseProposalDto
{
    /// <summary>
    /// Gets or sets the property identifier.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// Gets or sets the lease start date.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the lease end date.
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets or sets the proposed monthly rent.
    /// </summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>
    /// Gets or sets the proposed upfront payment.
    /// </summary>
    public decimal UpfrontPayment { get; set; }

    /// <summary>
    /// Gets or sets the proposed security deposit.
    /// </summary>
    public decimal SecurityDeposit { get; set; }

}
