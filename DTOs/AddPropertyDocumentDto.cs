namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for adding a document to a property.
/// </summary>
public class AddPropertyDocumentDto
{
    /// <summary>
    /// Gets or sets the document type identifier (1–5).
    /// </summary>
    public int DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the unique document reference number.
    /// </summary>
    public string DocumentNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the publicly accessible URL of the uploaded document file.
    /// </summary>
    public string DocumentUrl { get; set; } = null!;
}
