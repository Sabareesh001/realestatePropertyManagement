using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Represents a user verification request and its current status in the system.
/// </summary>
public partial class UserVerification
{
    /// <summary>
    /// Status indicating that a verification request is pending review.
    /// </summary>
    public const string StatusPending = "Pending";

    /// <summary>
    /// Status indicating that the user's verification has been approved.
    /// </summary>
    public const string StatusVerified = "Verified";

    /// <summary>
    /// Status indicating that the verification request was rejected.
    /// </summary>
    public const string StatusRejected = "Rejected";

    /// <summary>
    /// Status indicating that the user is unverified.
    /// </summary>
    public const string StatusUnverified = "Unverified";

    /// <summary>
    /// Gets or sets the unique identifier for the user verification record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the unique identifier of the user being verified.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the status of the verification.
    /// </summary>
    public string Status { get; set; } = StatusPending;

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
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

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

    /// <summary>
    /// Gets or sets the date and time when the user verification was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
