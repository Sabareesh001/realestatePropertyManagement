using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="RecordPaymentDtoValidator"/> class.
/// </summary>
[TestFixture]
public class RecordPaymentDtoValidatorTests
{
    private RecordPaymentDtoValidator _validator;

    /// <summary>
    /// Sets up the validator under test before each execution.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _validator = new RecordPaymentDtoValidator();
    }

    /// <summary>
    /// Verifies that a valid payment recording payload passes validation.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN1234567890",
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = Guid.NewGuid(), Amount = 500.00m },
                new() { ChargeId = Guid.NewGuid(), Amount = 700.50m }
            }
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that empty charge allocations fails validation.
    /// </summary>
    [Test]
    public async Task Validate_EmptyChargeAllocations_Fails()
    {
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN123",
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>()
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(dto.ChargeAllocations)), Is.True);
    }

    /// <summary>
    /// Verifies that charge allocation with empty ChargeId fails validation.
    /// </summary>
    [Test]
    public async Task Validate_EmptyChargeIdInAllocation_Fails()
    {
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN123",
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = Guid.Empty, Amount = 100 }
            }
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName.StartsWith(nameof(dto.ChargeAllocations))), Is.True);
    }

    /// <summary>
    /// Verifies that charge allocation with non-positive amount fails validation.
    /// </summary>
    /// <param name="amount">The invalid amount under test.</param>
    [TestCase(0)]
    [TestCase(-50)]
    public async Task Validate_InvalidAmountInAllocation_Fails(decimal amount)
    {
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN123",
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = Guid.NewGuid(), Amount = amount }
            }
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName.StartsWith(nameof(dto.ChargeAllocations))), Is.True);
    }

    /// <summary>
    /// Verifies that an invalid PaymentMethodId fails validation.
    /// </summary>
    /// <param name="paymentMethodId">The invalid payment method ID.</param>
    [TestCase(0)]
    [TestCase(-1)]
    public async Task Validate_InvalidPaymentMethodId_Fails(int paymentMethodId)
    {
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = paymentMethodId,
            TransactionRef = "TXN123",
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = Guid.NewGuid(), Amount = 100 }
            }
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(dto.PaymentMethodId)), Is.True);
    }

    /// <summary>
    /// Verifies that an empty TransactionRef fails validation.
    /// </summary>
    [Test]
    public async Task Validate_EmptyTransactionRef_Fails()
    {
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = string.Empty,
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = Guid.NewGuid(), Amount = 100 }
            }
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(dto.TransactionRef)), Is.True);
    }

    /// <summary>
    /// Verifies that an invalid CurrencyId fails validation.
    /// </summary>
    /// <param name="currencyId">The invalid currency ID.</param>
    [TestCase(0)]
    [TestCase(-2)]
    public async Task Validate_InvalidCurrencyId_Fails(int currencyId)
    {
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN123",
            CurrencyId = currencyId,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = Guid.NewGuid(), Amount = 100 }
            }
        };

        var result = await _validator.ValidateAsync(dto);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(dto.CurrencyId)), Is.True);
    }
}
