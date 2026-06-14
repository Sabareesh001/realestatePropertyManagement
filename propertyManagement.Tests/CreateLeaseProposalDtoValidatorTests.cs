using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="CreateLeaseProposalDtoValidator"/> class.
/// </summary>
[TestFixture]
public class CreateLeaseProposalDtoValidatorTests
{
    private CreateLeaseProposalDtoValidator _validator;

    /// <summary>
    /// Sets up the validator under test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new CreateLeaseProposalDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid lease proposal passes validation.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        var dto = new CreateLeaseProposalDto
        {
            PropertyId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)),
            MonthlyRent = 1200,
            UpfrontPayment = 2400,
            SecurityDeposit = 1500
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that an invalid property ID fails validation.
    /// </summary>
    [Test]
    public async Task Validate_PropertyIdZeroOrNegative_Fails()
    {
        var dto = new CreateLeaseProposalDto
        {
            PropertyId = 0
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Property ID must be greater than zero.")), Is.True);
    }

    /// <summary>
    /// Verifies that past start dates fail validation.
    /// </summary>
    [Test]
    public async Task Validate_StartDateInPast_Fails()
    {
        var dto = new CreateLeaseProposalDto
        {
            PropertyId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Start date cannot be in the past.")), Is.True);
    }

    /// <summary>
    /// Verifies that end dates before start dates fail validation.
    /// </summary>
    [Test]
    public async Task Validate_EndDateBeforeStartDate_Fails()
    {
        var dto = new CreateLeaseProposalDto
        {
            PropertyId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2))
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("End date must be after start date.")), Is.True);
    }
}
