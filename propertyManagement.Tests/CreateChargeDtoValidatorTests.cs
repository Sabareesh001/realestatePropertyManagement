using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="CreateChargeDtoValidator"/> class.
/// </summary>
[TestFixture]
public class CreateChargeDtoValidatorTests
{
    private CreateChargeDtoValidator _validator;

    /// <summary>
    /// Sets up the validator under test before each execution.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new CreateChargeDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid charge creation payload passes validation.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        var dto = new CreateChargeDto
        {
            ChargeTypeId = 1,
            Amount = 1200.50m,
            Description = "Monthly Rent for June",
            DueDate = DateTime.Today.AddDays(5)
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that a zero or negative charge type ID fails validation.
    /// </summary>
    /// <param name="chargeTypeId">The invalid charge type ID under test.</param>
    [TestCase(0)]
    [TestCase(-1)]
    public async Task Validate_InvalidChargeTypeId_Fails(int chargeTypeId)
    {
        var dto = new CreateChargeDto
        {
            ChargeTypeId = chargeTypeId,
            Amount = 1200.50m,
            Description = "Monthly Rent for June",
            DueDate = DateTime.Today.AddDays(5)
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(dto.ChargeTypeId)), Is.True);
    }

    /// <summary>
    /// Verifies that an amount less than or equal to zero fails validation.
    /// </summary>
    /// <param name="amount">The invalid amount under test.</param>
    [TestCase(0)]
    [TestCase(-100)]
    public async Task Validate_InvalidAmount_Fails(decimal amount)
    {
        var dto = new CreateChargeDto
        {
            ChargeTypeId = 1,
            Amount = amount,
            Description = "Invalid amount test",
            DueDate = DateTime.Today.AddDays(5)
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(dto.Amount)), Is.True);
    }

    /// <summary>
    /// Verifies that a missing (default) due date fails validation.
    /// </summary>
    [Test]
    public async Task Validate_EmptyDueDate_Fails()
    {
        var dto = new CreateChargeDto
        {
            ChargeTypeId = 1,
            Amount = 1200.50m,
            Description = "Missing due date test",
            DueDate = default
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(dto.DueDate)), Is.True);
    }
}
