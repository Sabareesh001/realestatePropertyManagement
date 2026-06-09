namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object containing parameters for verification approval or rejection.
/// </summary>
public class VerifyRequestDto
{
    /// <summary>
    /// Gets or sets the optional remarks/rejection reason for the verification action.
    /// </summary>
    public string? Remarks { get; set; }
}
