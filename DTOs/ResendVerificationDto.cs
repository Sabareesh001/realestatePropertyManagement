namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for requesting a resend of the email verification link.
/// </summary>
public class ResendVerificationDto
{
    /// <summary>
    /// The email address to resend the verification link to.
    /// </summary>
    public required string Email { get; set; }
}
