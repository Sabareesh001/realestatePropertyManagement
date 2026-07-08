using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using propertyManagement.Controllers;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="PropertyController"/> class.
/// </summary>
[TestFixture]
public class PropertyControllerTests
{
    private Mock<IPropertyService> _mockPropertyService;
    private Mock<IWebHostEnvironment> _mockEnv;
    private PropertyController _controller;

    /// <summary>
    /// Sets up the test environment with mocked dependencies.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockPropertyService = new Mock<IPropertyService>();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _controller = new PropertyController(_mockPropertyService.Object, _mockEnv.Object);
    }

    private void SetupUserContext(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    private void SetupAnonymousContext()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private static PropertyResponseDto BuildPropertyResponse(int id = 1, Guid? ownerId = null) =>
        new()
        {
            Id = id,
            OwnerId = ownerId ?? Guid.NewGuid(),
            Title = "Test Property",
            AddressLine = "123 Test St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500,
            VerificationStatusId = 1,
            AvailabilityStatusId = 1
        };

    // ── CreateProperty ─────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that CreateProperty returns 201 CreatedAtAction with the created property.
    /// </summary>
    [Test]
    public async Task CreateProperty_ValidRequest_ReturnsCreatedAtAction()
    {
        var userId = Guid.NewGuid();
        SetupUserContext(userId);

        var dto = new CreatePropertyDto
        {
            Title = "Test Property",
            AddressLine = "123 Test St",
            CityId = 1,
            MonthlyRent = 1000,
            UpfrontPayment = 2000,
            SecurityDeposit = 1500
        };

        var responseDto = BuildPropertyResponse(ownerId: userId);
        _mockPropertyService.Setup(s => s.CreatePropertyAsync(userId, dto)).ReturnsAsync(responseDto);

        var result = await _controller.CreateProperty(dto);

        var actionResult = result.Result as CreatedAtActionResult;
        Assert.That(actionResult, Is.Not.Null);
        Assert.That(actionResult!.ActionName, Is.EqualTo(nameof(PropertyController.GetPropertyById)));
        Assert.That(actionResult.RouteValues!["id"], Is.EqualTo(responseDto.Id));
        Assert.That(actionResult.Value, Is.EqualTo(responseDto));
    }

    /// <summary>
    /// Verifies that CreateProperty propagates InvalidOperationException when the owner is not verified.
    /// </summary>
    [Test]
    public void CreateProperty_UnverifiedOwner_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        SetupUserContext(userId);

        var dto = new CreatePropertyDto();
        _mockPropertyService.Setup(s => s.CreatePropertyAsync(userId, dto))
            .ThrowsAsync(new InvalidOperationException("User must be verified to post a property."));

        Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.CreateProperty(dto));
    }

    // ── GetPropertyById ────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that GetPropertyById returns 200 OK with the property when found.
    /// </summary>
    [Test]
    public async Task GetPropertyById_PropertyExists_ReturnsOk()
    {
        var propertyId = 5;
        SetupAnonymousContext();
        var responseDto = BuildPropertyResponse(id: propertyId);
        _mockPropertyService.Setup(s => s.GetPropertyByIdAsync(propertyId)).ReturnsAsync(responseDto);

        var result = await _controller.GetPropertyById(propertyId);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo(responseDto));
    }

    /// <summary>
    /// Verifies that GetPropertyById propagates KeyNotFoundException when the property is not found.
    /// </summary>
    [Test]
    public void GetPropertyById_PropertyNotFound_ThrowsKeyNotFoundException()
    {
        var propertyId = 99;
        SetupAnonymousContext();
        _mockPropertyService.Setup(s => s.GetPropertyByIdAsync(propertyId))
            .ThrowsAsync(new KeyNotFoundException("Property not found."));

        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _controller.GetPropertyById(propertyId));
    }

    // ── GetAllProperties ───────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that GetAllProperties returns 200 OK with the full list.
    /// </summary>
    [Test]
    public async Task GetAllProperties_ReturnsOkWithList()
    {
        var properties = new List<PropertyResponseDto>
        {
            BuildPropertyResponse(1),
            BuildPropertyResponse(2)
        };
        _mockPropertyService.Setup(s => s.GetAllPropertiesAsync()).ReturnsAsync(properties);

        var result = await _controller.GetAllProperties();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo(properties));
    }

    /// <summary>
    /// Verifies that GetAllProperties returns 200 OK with an empty list when no properties exist.
    /// </summary>
    [Test]
    public async Task GetAllProperties_NoProperties_ReturnsOkWithEmptyList()
    {
        _mockPropertyService.Setup(s => s.GetAllPropertiesAsync())
            .ReturnsAsync(new List<PropertyResponseDto>());

        var result = await _controller.GetAllProperties();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That((okResult!.Value as List<PropertyResponseDto>)!.Count, Is.EqualTo(0));
    }

    // ── GetMyProperties ────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that GetMyProperties returns 200 OK with properties belonging to the authenticated owner.
    /// </summary>
    [Test]
    public async Task GetMyProperties_AuthenticatedOwner_ReturnsOkWithOwnerProperties()
    {
        var userId = Guid.NewGuid();
        SetupUserContext(userId);

        var properties = new List<PropertyResponseDto>
        {
            BuildPropertyResponse(1, userId),
            BuildPropertyResponse(2, userId)
        };
        _mockPropertyService.Setup(s => s.GetPropertiesByOwnerIdAsync(userId)).ReturnsAsync(properties);

        var result = await _controller.GetMyProperties();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo(properties));
    }

    // ── UpdateProperty ─────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that UpdateProperty returns 200 OK with the updated property when the user is the owner.
    /// </summary>
    [Test]
    public async Task UpdateProperty_UserIsOwner_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var propertyId = 3;
        SetupUserContext(userId);

        var dto = new UpdatePropertyDto
        {
            Title = "Updated Title",
            AddressLine = "456 New St",
            CityId = 2,
            MonthlyRent = 1200,
            UpfrontPayment = 2400,
            SecurityDeposit = 1800
        };

        var responseDto = BuildPropertyResponse(propertyId, userId);
        responseDto.Title = "Updated Title";
        _mockPropertyService.Setup(s => s.UpdatePropertyAsync(userId, propertyId, dto)).ReturnsAsync(responseDto);

        var result = await _controller.UpdateProperty(propertyId, dto);

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.Value, Is.EqualTo(responseDto));
    }

    /// <summary>
    /// Verifies that UpdateProperty propagates KeyNotFoundException when the property is not found.
    /// </summary>
    [Test]
    public void UpdateProperty_PropertyNotFound_ThrowsKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        var propertyId = 99;
        SetupUserContext(userId);

        var dto = new UpdatePropertyDto();
        _mockPropertyService.Setup(s => s.UpdatePropertyAsync(userId, propertyId, dto))
            .ThrowsAsync(new KeyNotFoundException("Property not found."));

        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _controller.UpdateProperty(propertyId, dto));
    }

    /// <summary>
    /// Verifies that UpdateProperty propagates UnauthorizedAccessException when the user is not the owner.
    /// </summary>
    [Test]
    public void UpdateProperty_UserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();
        var propertyId = 3;
        SetupUserContext(userId);

        var dto = new UpdatePropertyDto();
        _mockPropertyService.Setup(s => s.UpdatePropertyAsync(userId, propertyId, dto))
            .ThrowsAsync(new UnauthorizedAccessException());

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _controller.UpdateProperty(propertyId, dto));
    }

    // ── DeleteProperty ─────────────────────────────────────────────────────────

    /// <summary>
    /// Verifies that DeleteProperty returns 204 NoContent when the user is the owner.
    /// </summary>
    [Test]
    public async Task DeleteProperty_UserIsOwner_ReturnsNoContent()
    {
        var userId = Guid.NewGuid();
        var propertyId = 7;
        SetupUserContext(userId);

        _mockPropertyService.Setup(s => s.DeletePropertyAsync(userId, propertyId)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteProperty(propertyId);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    /// <summary>
    /// Verifies that DeleteProperty propagates KeyNotFoundException when the property is not found.
    /// </summary>
    [Test]
    public void DeleteProperty_PropertyNotFound_ThrowsKeyNotFoundException()
    {
        var userId = Guid.NewGuid();
        var propertyId = 99;
        SetupUserContext(userId);

        _mockPropertyService.Setup(s => s.DeletePropertyAsync(userId, propertyId))
            .ThrowsAsync(new KeyNotFoundException("Property not found."));

        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _controller.DeleteProperty(propertyId));
    }

    /// <summary>
    /// Verifies that DeleteProperty propagates UnauthorizedAccessException when the user is not the owner.
    /// </summary>
    [Test]
    public void DeleteProperty_UserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();
        var propertyId = 7;
        SetupUserContext(userId);

        _mockPropertyService.Setup(s => s.DeletePropertyAsync(userId, propertyId))
            .ThrowsAsync(new UnauthorizedAccessException());

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _controller.DeleteProperty(propertyId));
    }
}
