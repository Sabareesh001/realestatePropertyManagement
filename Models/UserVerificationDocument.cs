using System;

namespace propertyManagement.Models;

/// <summary>
/// Represents the mapping between a user verification request and an associated document.
/// </summary>
public partial class UserVerificationDocument
{
    /// <summary>
    /// Gets or sets the unique identifier of the user verification request.
    /// </summary>
    public Guid UserVerificationId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the document.
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the associated user verification.
    /// </summary>
    public virtual UserVerification UserVerification { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property for the associated document.
    /// </summary>
    public virtual Document Document { get; set; } = null!;
}
