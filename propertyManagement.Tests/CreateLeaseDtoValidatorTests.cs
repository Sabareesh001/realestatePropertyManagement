using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="CreateLeaseDtoValidator"/> class.
/// </summary>
[TestFixture]
public class CreateLeaseDtoValidatorTests
{
    private CreateLeaseDtoValidator _validator;

    /// <summary>
    /// Sets up the validator under test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new CreateLeaseDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid lease creation payload passes validation.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        var dto = new CreateLeaseDto
        {
            TenantId = Guid.NewGuid(),
            PropertyId = 1,
            ProposalId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that an empty proposal ID fails validation.
    /// </summary>
    [Test]
    public async Task Validate_EmptyProposalId_Fails()
    {
        var dto = new CreateLeaseDto
        {
            TenantId = Guid.NewGuid(),
            PropertyId = 1,
            ProposalId = Guid.Empty,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.ErrorMessage.Contains("Proposal ID is required.")), Is.True);
    }
}
