using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for user-related operations.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="registerDto">The registration data containing email, password, and user details.</param>
    /// <returns>The created user response if successful.</returns>
    Task<UserResponseDto> RegisterAsync(RegisterDto registerDto);

    /// <summary>
    /// Authenticates a user with their email and password.
    /// </summary>
    /// <param name="loginDto">The login credentials containing email and password.</param>
    /// <returns>The authenticated user response if successful.</returns>
    Task<UserResponseDto> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>A list of all users.</returns>
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user details if found.</returns>
    Task<UserResponseDto> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Updates an existing user's information.
    /// </summary>
    /// <param name="id">The unique identifier of the user to update.</param>
    /// <param name="updateUserDto">The updated user information.</param>
    /// <returns>The updated user details.</returns>
    Task<UserResponseDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);

    /// <summary>
    /// Deletes a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    Task DeleteUserAsync(Guid id);

    /// <summary>
    /// Assigns the "Owner" role to the specified user without modifying existing roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The updated user response.</returns>
    Task<UserResponseDto> AssignOwnerRoleAsync(Guid userId);

    /// <summary>
    /// Confirms a user's email address using the verification hash sent to them.
    /// </summary>
    /// <param name="hash">The email verification hash.</param>
    Task VerifyEmailAsync(string hash);

    /// <summary>
    /// Regenerates and resends the email verification link for the given email address, if it belongs to an unverified account.
    /// </summary>
    /// <param name="email">The email address to resend the verification link to.</param>
    Task ResendVerificationEmailAsync(string email);
}
