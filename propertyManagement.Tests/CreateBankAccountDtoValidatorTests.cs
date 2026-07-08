using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="CreateBankAccountDtoValidator"/> class.
/// </summary>
[TestFixture]
public class CreateBankAccountDtoValidatorTests
{
    private CreateBankAccountDtoValidator _validator;

    /// <summary>
    /// Sets up the test environment by initializing the validator.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new CreateBankAccountDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid CreateBankAccountDto passes all validation rules.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "Chase Bank",
            AccountNumber = "1234567890",
            AccountHolderName = "John Doe",
            IfscCode = "CHAS0123456"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that an empty bank name fails validation.
    /// </summary>
    [Test]
    public async Task Validate_BankNameEmpty_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "",
            AccountNumber = "1234567890",
            AccountHolderName = "John Doe",
            IfscCode = "CHAS0123456"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Bank name is required."));
    }

    /// <summary>
    /// Verifies that a bank name exceeding 100 characters fails validation.
    /// </summary>
    [Test]
    public async Task Validate_BankNameTooLong_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = new string('A', 101),
            AccountNumber = "1234567890",
            AccountHolderName = "John Doe",
            IfscCode = "CHAS0123456"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Bank name cannot exceed 100 characters."));
    }

    /// <summary>
    /// Verifies that an empty account number fails validation.
    /// </summary>
    [Test]
    public async Task Validate_AccountNumberEmpty_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "Chase Bank",
            AccountNumber = "",
            AccountHolderName = "John Doe",
            IfscCode = "CHAS0123456"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Account number is required."));
    }

    /// <summary>
    /// Verifies that an account number exceeding the allowed 9–18 digit range fails validation.
    /// </summary>
    [Test]
    public async Task Validate_AccountNumberTooLong_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "Chase Bank",
            AccountNumber = new string('1', 51),
            AccountHolderName = "John Doe",
            IfscCode = "CHAS0123456"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Account number must contain only digits and be between 9 and 18 digits long."));
    }

    /// <summary>
    /// Verifies that an empty account holder name fails validation.
    /// </summary>
    [Test]
    public async Task Validate_AccountHolderNameEmpty_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "Chase Bank",
            AccountNumber = "1234567890",
            AccountHolderName = "",
            IfscCode = "CHAS0123456"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Account holder name is required."));
    }

    /// <summary>
    /// Verifies that an account holder name exceeding 100 characters fails validation.
    /// </summary>
    [Test]
    public async Task Validate_AccountHolderNameTooLong_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "Chase Bank",
            AccountNumber = "1234567890",
            AccountHolderName = new string('A', 101),
            IfscCode = "CHAS0123456"
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Account holder name cannot exceed 100 characters."));
    }

    /// <summary>
    /// Verifies that an empty IFSC code fails validation.
    /// </summary>
    [Test]
    public async Task Validate_IfscCodeEmpty_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "Chase Bank",
            AccountNumber = "1234567890",
            AccountHolderName = "John Doe",
            IfscCode = ""
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("IFSC code is required."));
    }

    /// <summary>
    /// Verifies that an IFSC code that is not exactly 11 characters fails validation.
    /// </summary>
    [Test]
    public async Task Validate_IfscCodeTooLong_Fails()
    {
        // Arrange
        var dto = new CreateBankAccountDto
        {
            BankName = "Chase Bank",
            AccountNumber = "1234567890",
            AccountHolderName = "John Doe",
            IfscCode = new string('A', 21)
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("IFSC code must be exactly 11 characters."));
    }
}
