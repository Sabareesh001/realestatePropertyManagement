using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing document details in response payloads.
/// </summary>
public class DocumentResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the document.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the document type identifier.
    /// </summary>
    public int? DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the unique document number.
    /// </summary>
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Gets or sets the document file URL.
    /// </summary>
    public string? DocumentUrl { get; set; }
}
