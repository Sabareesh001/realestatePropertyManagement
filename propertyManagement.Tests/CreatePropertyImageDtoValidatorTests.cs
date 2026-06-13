using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;
using System.Linq;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="CreatePropertyImageDtoValidator"/> class.
/// </summary>
[TestFixture]
public class CreatePropertyImageDtoValidatorTests
{
    private CreatePropertyImageDtoValidator _validator;

    /// <summary>
    /// Sets up the test validator instance.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new CreatePropertyImageDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid property image DTO passes all validation rules.
    /// </summary>
    [Test]
    public void Validate_ValidDto_ReturnsValid()
    {
        // Arrange
        var dto = new CreatePropertyImageDto
        {
            ImageUrl = "https://example.com/images/house.jpg",
            Description = "Front view of the house",
            DisplayOrder = 1
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that an empty or missing Image URL fails validation.
    /// </summary>
    [Test]
    public void Validate_ImageUrlEmpty_Fails()
    {
        // Arrange
        var dto = new CreatePropertyImageDto
        {
            ImageUrl = "",
            Description = "Front view",
            DisplayOrder = 1
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Image URL is required."));
    }

    /// <summary>
    /// Verifies that a negative display order fails validation.
    /// </summary>
    [Test]
    public void Validate_DisplayOrderNegative_Fails()
    {
        // Arrange
        var dto = new CreatePropertyImageDto
        {
            ImageUrl = "https://example.com/images/house.jpg",
            Description = "Front view",
            DisplayOrder = -1
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Display order cannot be negative."));
    }
}
