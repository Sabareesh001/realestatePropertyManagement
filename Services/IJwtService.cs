using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for generating and validating JSON Web Tokens (JWT).
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the specified user details.
    /// </summary>
    /// <param name="user">The user response DTO containing user information and roles.</param>
    /// <returns>A signed JWT token as a string.</returns>
    string GenerateToken(UserResponseDto user);
}
