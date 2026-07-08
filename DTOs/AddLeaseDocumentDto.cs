namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for a tenant adding an agreement document to a lease.
/// </summary>
public class AddLeaseDocumentDto
{
    /// <summary>
    /// Gets or sets the document type identifier.
    /// </summary>
    public int DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the optional unique document reference number.
    /// </summary>
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Gets or sets the publicly accessible URL of the uploaded document file.
    /// </summary>
    public string DocumentUrl { get; set; } = null!;
}
