using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="SubmitVerificationDtoValidator"/> class.
/// </summary>
[TestFixture]
public class SubmitVerificationDtoValidatorTests
{
    private SubmitVerificationDtoValidator _validator;

    /// <summary>
    /// Sets up the test environment by initializing the validator.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new SubmitVerificationDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid SubmitVerificationDto passes all validation rules.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        // Arrange
        var dto = new SubmitVerificationDto
        {
            Documents = new List<VerificationDocumentDto>
            {
                new()
                {
                    DocumentTypeId = 1,
                    DocumentNumber = "PAN12345",
                    DocumentUrl = "http://example.com/pan.jpg"
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that an empty documents list fails validation.
    /// </summary>
    [Test]
    public async Task Validate_DocumentsEmpty_Fails()
    {
        // Arrange
        var dto = new SubmitVerificationDto
        {
            Documents = new List<VerificationDocumentDto>()
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("At least one verification document is required."));
    }

    /// <summary>
    /// Verifies that a null documents list fails validation.
    /// </summary>
    [Test]
    public async Task Validate_DocumentsNull_Fails()
    {
        // Arrange
        var dto = new SubmitVerificationDto
        {
            Documents = null!
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("At least one verification document is required."));
    }

    /// <summary>
    /// Verifies that a verification document inside the collection failing validation triggers nested failure.
    /// </summary>
    [Test]
    public async Task Validate_NestedDocumentInvalid_Fails()
    {
        // Arrange
        var dto = new SubmitVerificationDto
        {
            Documents = new List<VerificationDocumentDto>
            {
                new()
                {
                    DocumentTypeId = 99, // Invalid
                    DocumentNumber = "", // Empty
                    DocumentUrl = "" // Empty
                }
            }
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Document type must be valid"));
        Assert.That(errors, Contains.Item("Document number is required."));
        Assert.That(errors, Contains.Item("Document URL is required."));
    }
}
