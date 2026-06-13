using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;
using propertyManagement.Validators;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="UpdatePropertyDtoValidator"/> class.
/// </summary>
[TestFixture]
public class UpdatePropertyDtoValidatorTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ICityRepository> _mockCityRepository;
    private UpdatePropertyDtoValidator _validator;

    /// <summary>
    /// Sets up the test environment by mocking dependencies and initializing the validator.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCityRepository = new Mock<ICityRepository>();
        _mockUnitOfWork.Setup(u => u.Cities).Returns(_mockCityRepository.Object);
        _validator = new UpdatePropertyDtoValidator(_mockUnitOfWork.Object);
    }

    /// <summary>
    /// Verifies that a valid property update DTO passes all validation rules.
    /// </summary>
    [Test]
    public async Task Validate_ValidDto_ReturnsValid()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House Updated",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.True);
    }

    /// <summary>
    /// Verifies that an empty or whitespace title fails validation.
    /// </summary>
    [Test]
    public async Task Validate_TitleEmpty_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Title is required."));
    }

    /// <summary>
    /// Verifies that a title exceeding 150 characters fails validation.
    /// </summary>
    [Test]
    public async Task Validate_TitleExceeds150Characters_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = new string('A', 151),
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Title cannot exceed 150 characters."));
    }

    /// <summary>
    /// Verifies that an empty address line fails validation.
    /// </summary>
    [Test]
    public async Task Validate_AddressLineEmpty_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Address line is required."));
    }

    /// <summary>
    /// Verifies that a zero or negative city ID fails validation.
    /// </summary>
    [Test]
    public async Task Validate_CityIdZeroOrNegative_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "123 Main St",
            CityId = 0,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("City ID must be valid"));
    }

    /// <summary>
    /// Verifies that a non-existent city ID fails validation.
    /// </summary>
    [Test]
    public async Task Validate_CityIdNonExistent_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "123 Main St",
            CityId = 999,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((City?)null);

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("The specified city does not exist."));
    }

    /// <summary>
    /// Verifies that a negative monthly rent fails validation.
    /// </summary>
    [Test]
    public async Task Validate_MonthlyRentNegative_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = -1,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Monthly rent cannot be negative."));
    }

    /// <summary>
    /// Verifies that a negative upfront payment fails validation.
    /// </summary>
    [Test]
    public async Task Validate_UpfrontPaymentNegative_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = -1,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Upfront payment cannot be negative."));
    }

    /// <summary>
    /// Verifies that both rent and upfront payment being zero simultaneously fails validation.
    /// </summary>
    [Test]
    public async Task Validate_BothRentAndUpfrontZero_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 0,
            UpfrontPayment = 0,
            SecurityDeposit = 1500
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Monthly rent and upfront payment cannot both be zero simultaneously."));
    }

    /// <summary>
    /// Verifies that a negative security deposit fails validation.
    /// </summary>
    [Test]
    public async Task Validate_SecurityDepositNegative_Fails()
    {
        // Arrange
        var dto = new UpdatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = -1
        };
        _mockCityRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new City { Id = 1 });

        // Act
        var result = await _validator.ValidateAsync(dto);

        // Assert
        Assert.That(result.IsValid, Is.False);
        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
        Assert.That(errors, Contains.Item("Security deposit cannot be negative."));
    }
}
