using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Represents a user verification request and its current status in the system.
/// </summary>
public partial class UserVerification
{
    /// <summary>
    /// Gets or sets the unique identifier for the user verification record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the unique identifier of the user being verified.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the status of the verification (e.g., "Pending", "Verified", "Rejected").
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Gets or sets optional remarks or rejection reason from the administrator.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the administrator/user who performed the verification.
    /// </summary>
    public Guid? VerifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the verification request was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the verification request was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the user being verified.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the navigation property for the administrator who verified the user.
    /// </summary>
    public virtual User? VerifiedByNavigation { get; set; }

    /// <summary>
    /// Gets or sets the collection of documents associated with this verification.
    /// </summary>
    public virtual ICollection<UserVerificationDocument> UserVerificationDocuments { get; set; } = new List<UserVerificationDocument>();
}
