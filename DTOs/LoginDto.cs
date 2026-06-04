using System.ComponentModel.DataAnnotations;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for user login.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    [EmailAddress]
    public required string Email { get; set; }

    /// <summary>
    /// The password for the user account.
    /// </summary>
    [MinLength(12)]
    public required string Password { get; set; }
}
