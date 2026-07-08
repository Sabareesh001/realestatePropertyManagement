using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;
using propertyManagement.Services;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="PropertyService"/> class.
/// </summary>
[TestFixture]
public class PropertyServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private Mock<IUserVerificationRepository> _mockUserVerificationRepository;
    private Mock<IPropertyImageRepository> _mockPropertyImageRepository;
    private PropertyService _propertyService;

    /// <summary>
    /// Sets up the test environment by mocking repository dependencies.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        _mockPropertyImageRepository = new Mock<IPropertyImageRepository>();

        _mockUnitOfWork.Setup(u => u.Properties).Returns(_mockPropertyRepository.Object);
        _mockUnitOfWork.Setup(u => u.UserVerifications).Returns(_mockUserVerificationRepository.Object);
        _mockUnitOfWork.Setup(u => u.PropertyImages).Returns(_mockPropertyImageRepository.Object);

        _propertyService = new PropertyService(_mockUnitOfWork.Object);
    }

    /// <summary>
    /// Verifies that CreatePropertyAsync succeeds when the owner is verified.
    /// </summary>
    [Test]
    public async Task CreatePropertyAsync_VerifiedOwner_Succeeds()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(ownerId)).ReturnsAsync(true);

        var dto = new CreatePropertyDto
        {
            Title = "Beautiful Mansion",
            Description = "Huge mansion with pool",
            AddressLine = "456 Luxury Way",
            CityId = 2,
            MonthlyRent = 5000,
            UpfrontPayment = 10000,
            SecurityDeposit = 7500,
            ThumbnailImgUrl = "http://example.com/image.jpg"
        };

        // Act
        var result = await _propertyService.CreatePropertyAsync(ownerId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo(dto.Title));
        Assert.That(result.Description, Is.EqualTo(dto.Description));
        Assert.That(result.AddressLine, Is.EqualTo(dto.AddressLine));
        Assert.That(result.CityId, Is.EqualTo(dto.CityId));
        Assert.That(result.MonthlyRent, Is.EqualTo(dto.MonthlyRent));
        Assert.That(result.UpfrontPayment, Is.EqualTo(dto.UpfrontPayment));
        Assert.That(result.SecurityDeposit, Is.EqualTo(dto.SecurityDeposit));
        Assert.That(result.ThumbnailImgUrl, Is.EqualTo(dto.ThumbnailImgUrl));
        Assert.That(result.VerificationStatusId, Is.EqualTo((int)PropertyVerificationStatus.Draft));
        Assert.That(result.AvailabilityStatusId, Is.EqualTo((int)PropertyAvailabilityStatus.Unavailable));

        _mockPropertyRepository.Verify(r => r.CreateAsync(It.IsAny<Property>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that CreatePropertyAsync throws InvalidOperationException when the owner is not verified.
    /// </summary>
    [Test]
    public void CreatePropertyAsync_UnverifiedOwner_ThrowsInvalidOperationException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(ownerId)).ReturnsAsync(false);

        var dto = new CreatePropertyDto();

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _propertyService.CreatePropertyAsync(ownerId, dto));
    }

    /// <summary>
    /// Verifies that GetPropertyByIdAsync returns the property response DTO when found.
    /// </summary>
    [Test]
    public async Task GetPropertyByIdAsync_PropertyExists_ReturnsProperty()
    {
        // Arrange
        var propertyId = 10;
        var property = new Property
        {
            Id = propertyId,
            OwnerId = Guid.NewGuid(),
            Title = "Cosy Apartment",
            AddressLine = "789 Cosy Ave",
            CityId = 3,
            MonthlyRent = 1200,
            UpfrontPayment = 1200,
            SecurityDeposit = 1200,
            VerificationStatusId = PropertyVerificationStatus.Verified
        };
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

        // Act
        var result = await _propertyService.GetPropertyByIdAsync(propertyId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(propertyId));
        Assert.That(result.Title, Is.EqualTo(property.Title));
    }

    /// <summary>
    /// Verifies that GetPropertyByIdAsync throws KeyNotFoundException when property does not exist.
    /// </summary>
    [Test]
    public void GetPropertyByIdAsync_PropertyDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var propertyId = 10;
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync((Property?)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _propertyService.GetPropertyByIdAsync(propertyId));
    }

    /// <summary>
    /// Verifies that GetAllPropertiesAsync returns all properties.
    /// </summary>
    [Test]
    public async Task GetAllPropertiesAsync_ReturnsAllProperties()
    {
        // Arrange
        var properties = new List<Property>
        {
            new() { Id = 1, OwnerId = Guid.NewGuid(), Title = "Prop 1", AddressLine = "Add 1", VerificationStatusId = PropertyVerificationStatus.Verified },
            new() { Id = 2, OwnerId = Guid.NewGuid(), Title = "Prop 2", AddressLine = "Add 2", VerificationStatusId = PropertyVerificationStatus.Verified }
        };
        _mockPropertyRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(properties);

        // Act
        var result = (await _propertyService.GetAllPropertiesAsync()).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[0].Title, Is.EqualTo("Prop 1"));
        Assert.That(result[1].Id, Is.EqualTo(2));
        Assert.That(result[1].Title, Is.EqualTo("Prop 2"));
    }

    /// <summary>
    /// Verifies that UpdatePropertyAsync succeeds when the user is the owner.
    /// </summary>
    [Test]
    public async Task UpdatePropertyAsync_UserIsOwner_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var propertyId = 5;
        var property = new Property
        {
            Id = propertyId,
            OwnerId = userId,
            Title = "Old Title",
            AddressLine = "Old Address",
            CityId = 1,
            MonthlyRent = 800,
            UpfrontPayment = 800,
            SecurityDeposit = 800
        };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

        var dto = new UpdatePropertyDto
        {
            Title = "New Title",
            Description = "New Desc",
            AddressLine = "New Address",
            CityId = 2,
            MonthlyRent = 900,
            UpfrontPayment = 900,
            SecurityDeposit = 900,
            ThumbnailImgUrl = "http://example.com/new.jpg"
        };

        // Act
        var result = await _propertyService.UpdatePropertyAsync(userId, propertyId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("New Title"));
        Assert.That(result.Description, Is.EqualTo("New Desc"));
        Assert.That(result.AddressLine, Is.EqualTo("New Address"));
        Assert.That(result.CityId, Is.EqualTo(2));
        Assert.That(result.MonthlyRent, Is.EqualTo(900));

        _mockPropertyRepository.Verify(r => r.UpdateAsync(property), Times.Once);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that UpdatePropertyAsync throws KeyNotFoundException if property does not exist.
    /// </summary>
    [Test]
    public void UpdatePropertyAsync_PropertyNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var propertyId = 5;
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync((Property?)null);

        var dto = new UpdatePropertyDto();

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _propertyService.UpdatePropertyAsync(userId, propertyId, dto));
    }

    /// <summary>
    /// Verifies that UpdatePropertyAsync throws UnauthorizedAccessException if the user is not the owner.
    /// </summary>
    [Test]
    public void UpdatePropertyAsync_UserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var propertyId = 5;
        var property = new Property
        {
            Id = propertyId,
            OwnerId = otherUserId,
            Title = "Old Title"
        };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

        var dto = new UpdatePropertyDto();

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _propertyService.UpdatePropertyAsync(userId, propertyId, dto));
    }

    /// <summary>
    /// Verifies that DeletePropertyAsync succeeds when the user is the owner.
    /// </summary>
    [Test]
    public async Task DeletePropertyAsync_UserIsOwner_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var propertyId = 5;
        var property = new Property
        {
            Id = propertyId,
            OwnerId = userId
        };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

        // Act
        await _propertyService.DeletePropertyAsync(userId, propertyId);

        // Assert
        _mockPropertyRepository.Verify(r => r.DeleteAsync(propertyId), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that DeletePropertyAsync throws KeyNotFoundException if property does not exist.
    /// </summary>
    [Test]
    public void DeletePropertyAsync_PropertyNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var propertyId = 5;
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync((Property?)null);

        // Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _propertyService.DeletePropertyAsync(userId, propertyId));
    }

    /// <summary>
    /// Verifies that DeletePropertyAsync throws UnauthorizedAccessException if the user is not the owner.
    /// </summary>
    [Test]
    public void DeletePropertyAsync_UserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var propertyId = 5;
        var property = new Property
        {
            Id = propertyId,
            OwnerId = otherUserId
        };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

        // Act & Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _propertyService.DeletePropertyAsync(userId, propertyId));
    }

    /// <summary>
    /// Verifies that GetPropertiesByOwnerIdAsync returns only properties belonging to the specified owner.
    /// </summary>
    [Test]
    public async Task GetPropertiesByOwnerIdAsync_ReturnsOwnerProperties()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var properties = new List<Property>
        {
            new() { Id = 1, OwnerId = ownerId, Title = "Prop 1" },
            new() { Id = 2, OwnerId = ownerId, Title = "Prop 2" }
        };
        _mockPropertyRepository.Setup(r => r.GetPropertiesByOwnerIdAsync(ownerId)).ReturnsAsync(properties);

        // Act
        var result = (await _propertyService.GetPropertiesByOwnerIdAsync(ownerId)).ToList();

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].Id, Is.EqualTo(1));
        Assert.That(result[0].OwnerId, Is.EqualTo(ownerId));
        Assert.That(result[1].Id, Is.EqualTo(2));
        Assert.That(result[1].OwnerId, Is.EqualTo(ownerId));
    }

    /// <summary>
    /// Verifies that CreatePropertyAsync successfully saves nested property images.
    /// </summary>
    [Test]
    public async Task CreatePropertyAsync_WithImages_Succeeds()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(ownerId)).ReturnsAsync(true);

        var dto = new CreatePropertyDto
        {
            Title = "Beautiful House",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500,
            PropertyImages = new List<CreatePropertyImageDto>
            {
                new() { ImageUrl = "https://example.com/img1.jpg", Description = "Living Room", DisplayOrder = 0 },
                new() { ImageUrl = "https://example.com/img2.jpg", Description = "Bedroom", DisplayOrder = 1 }
            }
        };

        // Act
        var result = await _propertyService.CreatePropertyAsync(ownerId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PropertyImages.Count, Is.EqualTo(2));
        var images = result.PropertyImages.ToList();
        Assert.That(images[0].ImageUrl, Is.EqualTo("https://example.com/img1.jpg"));
        Assert.That(images[1].ImageUrl, Is.EqualTo("https://example.com/img2.jpg"));

        _mockPropertyRepository.Verify(r => r.CreateAsync(It.Is<Property>(p => p.PropertyImages.Count == 2)), Times.Once);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that UpdatePropertyAsync correctly reconciles property images.
    /// </summary>
    [Test]
    public async Task UpdatePropertyAsync_WithReconciledImages_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var propertyId = 5;
        var existingImgId1 = Guid.NewGuid();
        var existingImgId2 = Guid.NewGuid();

        var property = new Property
        {
            Id = propertyId,
            OwnerId = userId,
            Title = "Old Title",
            AddressLine = "Old Address",
            CityId = 1,
            MonthlyRent = 800,
            UpfrontPayment = 800,
            SecurityDeposit = 800,
            PropertyImages = new List<PropertyImage>
            {
                new() { Id = existingImgId1, PropertyId = propertyId, ImageUrl = "https://example.com/img1.jpg", Description = "Old Living", DisplayOrder = 0 },
                new() { Id = existingImgId2, PropertyId = propertyId, ImageUrl = "https://example.com/img2.jpg", Description = "Old Bed", DisplayOrder = 1 }
            }
        };

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

        var dto = new UpdatePropertyDto
        {
            Title = "New Title",
            AddressLine = "New Address",
            CityId = 1,
            MonthlyRent = 900,
            UpfrontPayment = 900,
            SecurityDeposit = 900,
            PropertyImages = new List<PropertyImageDto>
            {
                // Update existingImgId1, delete existingImgId2, add a new image
                new() { Id = existingImgId1, ImageUrl = "https://example.com/img1_updated.jpg", Description = "Updated Living", DisplayOrder = 0 },
                new() { ImageUrl = "https://example.com/img3.jpg", Description = "Kitchen", DisplayOrder = 2 }
            }
        };

        // Act
        var result = await _propertyService.UpdatePropertyAsync(userId, propertyId, dto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PropertyImages.Count, Is.EqualTo(2));
        var images = result.PropertyImages.ToList();
        Assert.That(images[0].ImageUrl, Is.EqualTo("https://example.com/img1_updated.jpg"));
        Assert.That(images[1].ImageUrl, Is.EqualTo("https://example.com/img3.jpg"));

        _mockPropertyImageRepository.Verify(r => r.DeleteAsync(existingImgId2), Times.Once);
        _mockPropertyImageRepository.Verify(r => r.UpdateAsync(It.Is<PropertyImage>(pi => pi.Id == existingImgId1)), Times.Once);
        _mockPropertyImageRepository.Verify(r => r.CreateAsync(It.Is<PropertyImage>(pi => pi.ImageUrl == "https://example.com/img3.jpg")), Times.Once);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
}

