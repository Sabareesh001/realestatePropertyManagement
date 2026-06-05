using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the JwtService class.
/// </summary>
[TestFixture]
public class JwtServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private JwtService _jwtService;

    /// <summary>
    /// Sets up the test environment before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _jwtService = new JwtService(_mockConfiguration.Object);
    }

    /// <summary>
    /// Verifies that GenerateToken throws InvalidOperationException when the Jwt:Secret is missing from configuration.
    /// </summary>
    [Test]
    public void GenerateToken_SecretMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        _mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns((string?)null);
        var user = new UserResponseDto { Id = Guid.NewGuid(), Email = "user@example.com" };

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _jwtService.GenerateToken(user));
        Assert.That(ex.Message, Is.EqualTo("JWT Secret is not configured."));
    }

    /// <summary>
    /// Verifies that GenerateToken generates a valid token when all JWT configuration is valid and the user has a role.
    /// </summary>
    [Test]
    public void GenerateToken_ValidUserWithRole_ReturnsValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserResponseDto
        {
            Id = userId,
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = new RoleResponseDto { Id = 1, Name = "Admin" }
        };

        _mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("SuperSecretKeyThatIsLongEnoughToBeSecure123!");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("audience");
        _mockConfiguration.Setup(c => c["Jwt:ExpiryInMinutes"]).Returns("30");

        // Act
        var tokenString = _jwtService.GenerateToken(user);

        // Assert
        Assert.That(tokenString, Is.Not.Null.Or.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(tokenString) as JwtSecurityToken;

        Assert.That(jsonToken, Is.Not.Null);
        Assert.That(jsonToken.Issuer, Is.EqualTo("issuer"));
        Assert.That(jsonToken.Audiences, Contains.Item("audience"));

        var nameIdentifierClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var emailClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var givenNameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        var surnameClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        Assert.That(nameIdentifierClaim, Is.EqualTo(userId.ToString()));
        Assert.That(emailClaim, Is.EqualTo("user@example.com"));
        Assert.That(givenNameClaim, Is.EqualTo("John"));
        Assert.That(surnameClaim, Is.EqualTo("Doe"));
        Assert.That(roleClaim, Is.EqualTo("Admin"));
    }

    /// <summary>
    /// Verifies that GenerateToken generates a valid token when user has no role.
    /// </summary>
    [Test]
    public void GenerateToken_ValidUserWithoutRole_ReturnsValidTokenWithoutRoleClaim()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserResponseDto
        {
            Id = userId,
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = null
        };

        _mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("SuperSecretKeyThatIsLongEnoughToBeSecure123!");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("audience");
        _mockConfiguration.Setup(c => c["Jwt:ExpiryInMinutes"]).Returns("30");

        // Act
        var tokenString = _jwtService.GenerateToken(user);

        // Assert
        Assert.That(tokenString, Is.Not.Null.Or.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(tokenString) as JwtSecurityToken;

        Assert.That(jsonToken, Is.Not.Null);
        var roleClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        Assert.That(roleClaim, Is.Null);
    }

    /// <summary>
    /// Verifies that GenerateToken defaults the expiration time to 60 minutes if Jwt:ExpiryInMinutes is not a valid double.
    /// </summary>
    [Test]
    public void GenerateToken_InvalidExpiryConfigValue_DefaultsTo60Minutes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserResponseDto
        {
            Id = userId,
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = null
        };

        _mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("SuperSecretKeyThatIsLongEnoughToBeSecure123!");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("audience");
        _mockConfiguration.Setup(c => c["Jwt:ExpiryInMinutes"]).Returns("invalid_double");

        // Act
        var tokenString = _jwtService.GenerateToken(user);

        // Assert
        Assert.That(tokenString, Is.Not.Null.Or.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(tokenString) as JwtSecurityToken;

        Assert.That(jsonToken, Is.Not.Null);
        
        // Expiry should be roughly 60 minutes from now.
        var expectedExpiry = DateTime.UtcNow.AddMinutes(60);
        var difference = jsonToken.ValidTo - expectedExpiry;
        Assert.That(Math.Abs(difference.TotalSeconds), Is.LessThan(10));
    }
}
