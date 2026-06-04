namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for updating user information.
/// </summary>
public class UpdateUserDto
{
    /// <summary>
    /// The first name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// The phone number of the user.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// The date of birth of the user.
    /// </summary>
    public DateOnly? DateOfBirth { get; set; }
}
