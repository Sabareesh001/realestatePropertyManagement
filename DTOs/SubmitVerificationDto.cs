using System.Collections.Generic;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for submitting a user verification request
/// </summary>
public class SubmitVerificationDto
{
    /// <summary>
    /// Gets or sets the collection of documents accompanying the verification request.
    /// </summary>
    public List<VerificationDocumentDto> Documents { get; set; } = new();
}
