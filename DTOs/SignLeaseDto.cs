using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for tenant signature submission.
/// </summary>
public class SignLeaseDto
{
    /// <summary>
    /// Gets or sets the URL of the tenant-signed agreement document.
    /// </summary>
    public string SignedAgreementDocumentUrl { get; set; } = null!;
}
