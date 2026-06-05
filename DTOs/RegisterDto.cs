namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for user registration.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// The email address of the user.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// The password for the user account.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// The first name of the user.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// The phone number of the user.
    /// </summary>
    public required string Phone { get; set; }

    /// <summary>
    /// The date of birth of the user.
    /// </summary>
    public required DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// The unique identifier of the role to assign to the user.
    /// </summary>
    public required int RoleId { get; set; }
}
