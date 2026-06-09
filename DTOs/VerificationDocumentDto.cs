namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for document details uploaded during user verification.
/// </summary>
public class VerificationDocumentDto
{
    /// <summary>
    /// Gets or sets the document type identifier.
    /// </summary>
    public int DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the unique number/identifier of the document.
    /// </summary>
    public string DocumentNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the public URL of the uploaded document file.
    /// </summary>
    public string DocumentUrl { get; set; } = null!;
}
