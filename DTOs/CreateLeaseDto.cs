using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for creating a new lease.
/// </summary>
public class CreateLeaseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the tenant.
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Gets or sets the property identifier.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// Gets or sets the rent request (proposal) identifier associated with this lease.
    /// </summary>
    public Guid ProposalId { get; set; }

    /// <summary>
    /// Gets or sets the lease start date.
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Gets or sets the lease end date.
    /// </summary>
    public DateOnly EndDate { get; set; }

    /// <summary>
    /// Gets or sets the monthly rent.
    /// </summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>
    /// Gets or sets the upfront payment.
    /// </summary>
    public decimal UpfrontPayment { get; set; }

    /// <summary>
    /// Gets or sets the security deposit.
    /// </summary>
    public decimal SecurityDeposit { get; set; }


    /// <summary>
    /// Gets or sets the URL of the agreement document. Required if submitting the lease.
    /// </summary>
    public string? AgreementDocumentUrl { get; set; }

    /// <summary>
    /// Gets or sets the optional collection of additional documents associated with the lease.
    /// </summary>
    public List<LeaseDocumentDto>? Documents { get; set; }
}
