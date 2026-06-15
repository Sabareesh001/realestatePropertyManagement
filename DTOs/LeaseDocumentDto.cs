using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a document associated with a lease.
/// </summary>
public class LeaseDocumentDto
{
    /// <summary>
    /// Gets or sets the document type identifier.
    /// </summary>
    public int DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the optional unique number/identifier of the document.
    /// </summary>
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Gets or sets the public URL of the uploaded document file.
    /// </summary>
    public string DocumentUrl { get; set; } = null!;
}
