using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="VerifyRequestDtoValidator"/> class.
/// </summary>
[TestFixture]
public class VerifyRequestDtoValidatorTests
{
    private VerifyRequestDtoValidator _validator;

    /// <summary>
    /// Sets up the test environment by initializing the validator.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new VerifyRequestDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid VerifyRequestDto passes all validation rules.
    /// </summary>
    [Test]
    public async Task Validate_ValidRemarks_ReturnsValid()
    {
        // Arrange
        var dto = new VerifyRequestDto
        {
            Remarks = "Approved: documents are correct."
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that null remarks pass validation.
    /// </summary>
    [Test]
    public async Task Validate_RemarksNull_ReturnsValid()
    {
        // Arrange
        var dto = new VerifyRequestDto
        {
            Remarks = null
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that empty remarks pass validation.
    /// </summary>
    [Test]
    public async Task Validate_RemarksEmpty_ReturnsValid()
    {
        // Arrange
        var dto = new VerifyRequestDto
        {
            Remarks = ""
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that remarks exceeding 500 characters fail validation.
    /// </summary>
    [Test]
    public async Task Validate_RemarksTooLong_Fails()
    {
        // Arrange
        var dto = new VerifyRequestDto
        {
            Remarks = new string('A', 501)
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Remarks cannot exceed 500 characters."));
    }
}
