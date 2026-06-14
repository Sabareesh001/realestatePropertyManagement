using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Entity representing a type of document in the property management system.
/// </summary>
public partial class DocumentType
{
    /// <summary>
    /// The unique identifier for the Pan card document type.
    /// </summary>
    public const int PanCard = 1;

    /// <summary>
    /// The unique identifier for the Property Deed document type.
    /// </summary>
    public const int PropertyDeed = 2;

    /// <summary>
    /// The unique identifier for the Salary Slip document type.
    /// </summary>
    public const int SalarySlip = 3;

    /// <summary>
    /// The unique identifier for the Lease Agreement document type.
    /// </summary>
    public const int LeaseAgreement = 4;

    /// <summary>
    /// The unique identifier for the Signed Lease Agreement document type.
    /// </summary>
    public const int SignedLeaseAgreement = 5;


    /// <summary>
    /// Gets or sets the unique identifier of the document type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the document type.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the documents associated with this document type.
    /// </summary>
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    /// <summary>
    /// Gets or sets the date and time when the document type was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the document type was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the document type was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
