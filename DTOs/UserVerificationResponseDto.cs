using System;
using System.Collections.Generic;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing user verification status details.
/// </summary>
public class UserVerificationResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the verification record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the verification status (e.g., "Pending", "Verified", "Rejected").
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// Gets or sets optional remarks from the administrator who reviewed the request.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the administrator who performed the verification.
    /// </summary>
    public Guid? VerifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the request was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the request was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of documents associated with this verification request.
    /// </summary>
    public List<DocumentResponseDto> Documents { get; set; } = new();
}
