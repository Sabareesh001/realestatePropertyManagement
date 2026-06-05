using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for generating and validating JSON Web Tokens (JWT).
/// </summary>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtService"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration to retrieve JWT settings.</param>
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT token for the specified user details.
    /// </summary>
    /// <param name="user">The user response DTO containing user information and roles.</param>
    /// <returns>A signed JWT token as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when JWT configuration is missing.</exception>
    public string GenerateToken(UserResponseDto user)
    {
        var secretKey = _configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT Secret is not configured.");
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var expiryMinutesStr = _configuration["Jwt:ExpiryInMinutes"];
        var expiryMinutes = double.TryParse(expiryMinutesStr, out var minutes) ? minutes : 60;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName)
        };

        if (user.Role != null && !string.IsNullOrEmpty(user.Role.Name))
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
