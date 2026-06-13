using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="VerificationDocumentDtoValidator"/> class.
/// </summary>
[TestFixture]
public class VerificationDocumentDtoValidatorTests
{
    private VerificationDocumentDtoValidator _validator;

    /// <summary>
    /// Sets up the test environment by initializing the validator.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new VerificationDocumentDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid VerificationDocumentDto passes all validation rules.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        // Arrange
        var dto = new VerificationDocumentDto
        {
            DocumentTypeId = 1,
            DocumentNumber = "PAN12345",
            DocumentUrl = "http://example.com/pan.jpg"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that an invalid DocumentTypeId fails validation.
    /// </summary>
    [TestCase(0)]
    [TestCase(5)]
    [TestCase(-1)]
    public async Task Validate_DocumentTypeIdInvalid_Fails(int documentTypeId)
    {
        // Arrange
        var dto = new VerificationDocumentDto
        {
            DocumentTypeId = documentTypeId,
            DocumentNumber = "PAN12345",
            DocumentUrl = "http://example.com/pan.jpg"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Document type must be valid (1 = PanCard, 2 = PropertyDeed, 3 = SalarySlip, 4 = LeaseAgreement)."));
    }

    /// <summary>
    /// Verifies that an empty document number fails validation.
    /// </summary>
    [Test]
    public async Task Validate_DocumentNumberEmpty_Fails()
    {
        // Arrange
        var dto = new VerificationDocumentDto
        {
            DocumentTypeId = 1,
            DocumentNumber = "",
            DocumentUrl = "http://example.com/pan.jpg"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Document number is required."));
    }

    /// <summary>
    /// Verifies that a document number exceeding 50 characters fails validation.
    /// </summary>
    [Test]
    public async Task Validate_DocumentNumberTooLong_Fails()
    {
        // Arrange
        var dto = new VerificationDocumentDto
        {
            DocumentTypeId = 1,
            DocumentNumber = new string('A', 51),
            DocumentUrl = "http://example.com/pan.jpg"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Document number cannot exceed 50 characters."));
    }

    /// <summary>
    /// Verifies that an empty document URL fails validation.
    /// </summary>
    [Test]
    public async Task Validate_DocumentUrlEmpty_Fails()
    {
        // Arrange
        var dto = new VerificationDocumentDto
        {
            DocumentTypeId = 1,
            DocumentNumber = "PAN12345",
            DocumentUrl = ""
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Document URL is required."));
    }
}
