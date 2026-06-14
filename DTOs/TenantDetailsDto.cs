using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing detailed tenant information for rent requests.
/// </summary>
public class TenantDetailsDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the tenant user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the first name of the tenant.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the last name of the tenant.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the email address of the tenant.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Gets or sets the phone number of the tenant.
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// Gets or sets the occupation of the tenant.
    /// </summary>
    public string? Occupation { get; set; }

    /// <summary>
    /// Gets or sets the monthly income of the tenant.
    /// </summary>
    public decimal? MonthlyIncome { get; set; }
}
