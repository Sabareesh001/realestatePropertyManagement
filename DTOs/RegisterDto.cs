using System.ComponentModel.DataAnnotations;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for user registration.
/// </summary>
public class RegisterDto
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

    /// <summary>
    /// The first name of the user.
    /// </summary>
    [Required]
    public required string FirstName { get; set; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    [Required]
    public required string LastName { get; set; }

    /// <summary>
    /// The phone number of the user.
    /// </summary>
    [Required]
    public required string Phone { get; set; }

    /// <summary>
    /// The date of birth of the user.
    /// </summary>
    [Required]
    public DateOnly DateOfBirth { get; set; }
}
