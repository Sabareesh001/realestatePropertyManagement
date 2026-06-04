namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for user response payloads.
/// </summary>
public class UserResponseDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The first name of the user.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// The phone number of the user.
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// The date of birth of the user.
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// The timestamp when the user was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// The timestamp when the user was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
