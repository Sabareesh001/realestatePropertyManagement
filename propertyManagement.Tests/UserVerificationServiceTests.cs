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

[TestFixture]
public class UserVerificationServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IUserVerificationRepository> _mockVerificationRepository;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private Mock<ILeaseProposalRepository> _mockLeaseProposalRepository;
    private UserVerificationService _verificationService;
    private PropertyService _propertyService;
    private LeaseProposalService _leaseProposalService;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockVerificationRepository = new Mock<IUserVerificationRepository>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockLeaseProposalRepository = new Mock<ILeaseProposalRepository>();

        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(u => u.UserVerifications).Returns(_mockVerificationRepository.Object);
        _mockUnitOfWork.Setup(u => u.Properties).Returns(_mockPropertyRepository.Object);
        _mockUnitOfWork.Setup(u => u.LeaseProposals).Returns(_mockLeaseProposalRepository.Object);

        _verificationService = new UserVerificationService(_mockUnitOfWork.Object);
        _propertyService = new PropertyService(_mockUnitOfWork.Object);
        _leaseProposalService = new LeaseProposalService(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task SubmitForVerificationAsync_ValidRequest_CreatesVerification()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockVerificationRepository.Setup(r => r.GetLatestVerificationByUserIdAsync(userId))
            .ReturnsAsync((UserVerification?)null);

        var submitDto = new SubmitVerificationDto
        {
            Documents = new List<VerificationDocumentDto>
            {
                new() { DocumentTypeId = 1, DocumentNumber = "DOC123", DocumentUrl = "http://example.com/doc1" }
            }
        };

        // Act
        var result = await _verificationService.SubmitForVerificationAsync(userId, submitDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo("Pending"));
        Assert.That(result.Documents, Has.Count.EqualTo(1));
        Assert.That(result.Documents[0].DocumentNumber, Is.EqualTo("DOC123"));
        Assert.That(user.VerificationStatusId, Is.EqualTo(UserVerificationStatus.Pending));

        _mockVerificationRepository.Verify(r => r.CreateAsync(It.IsAny<UserVerification>()), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task SubmitForVerificationAsync_RequestAlreadyPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var existingVerification = new UserVerification { Status = "Pending" };
        _mockVerificationRepository.Setup(r => r.GetLatestVerificationByUserIdAsync(userId))
            .ReturnsAsync(existingVerification);

        var submitDto = new SubmitVerificationDto();

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _verificationService.SubmitForVerificationAsync(userId, submitDto));
        Assert.That(ex.Message, Is.EqualTo("A verification request is already pending."));
    }

    [Test]
    public async Task SubmitForVerificationAsync_AlreadyVerified_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId };
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var existingVerification = new UserVerification { Status = "Verified" };
        _mockVerificationRepository.Setup(r => r.GetLatestVerificationByUserIdAsync(userId))
            .ReturnsAsync(existingVerification);

        var submitDto = new SubmitVerificationDto();

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _verificationService.SubmitForVerificationAsync(userId, submitDto));
        Assert.That(ex.Message, Is.EqualTo("User is already verified."));
    }

    [Test]
    public async Task ApproveVerificationAsync_PendingRequest_ApprovesVerification()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var verificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, VerificationStatusId = UserVerificationStatus.Pending };
        var verification = new UserVerification { Id = verificationId, UserId = userId, Status = "Pending" };

        _mockVerificationRepository.Setup(r => r.GetByIdAsync(verificationId)).ReturnsAsync(verification);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var verifyDto = new VerifyRequestDto { Remarks = "Approved remarks" };

        // Act
        var result = await _verificationService.ApproveVerificationAsync(adminId, verificationId, verifyDto);

        // Assert
        Assert.That(result.Status, Is.EqualTo("Verified"));
        Assert.That(result.VerifiedBy, Is.EqualTo(adminId));
        Assert.That(result.Remarks, Is.EqualTo("Approved remarks"));
        Assert.That(user.VerificationStatusId, Is.EqualTo(UserVerificationStatus.Verified));

        _mockVerificationRepository.Verify(r => r.UpdateAsync(verification), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task RejectVerificationAsync_PendingRequest_RejectsVerification()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var verificationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, VerificationStatusId = UserVerificationStatus.Pending };
        var verification = new UserVerification { Id = verificationId, UserId = userId, Status = "Pending" };

        _mockVerificationRepository.Setup(r => r.GetByIdAsync(verificationId)).ReturnsAsync(verification);
        _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var verifyDto = new VerifyRequestDto { Remarks = "Invalid docs" };

        // Act
        var result = await _verificationService.RejectVerificationAsync(adminId, verificationId, verifyDto);

        // Assert
        Assert.That(result.Status, Is.EqualTo("Rejected"));
        Assert.That(result.VerifiedBy, Is.EqualTo(adminId));
        Assert.That(result.Remarks, Is.EqualTo("Invalid docs"));
        Assert.That(user.VerificationStatusId, Is.EqualTo(UserVerificationStatus.Rejected));

        _mockVerificationRepository.Verify(r => r.UpdateAsync(verification), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreatePropertyAsync_VerifiedUser_Succeeds()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        _mockVerificationRepository.Setup(r => r.IsUserVerifiedAsync(ownerId)).ReturnsAsync(true);

        var propertyDto = new CreatePropertyDto
        {
            Title = "Luxury Villa",
            AddressLine = "123 Main St",
            CityId = 1,
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500
        };

        // Act
        var result = await _propertyService.CreatePropertyAsync(ownerId, propertyDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("Luxury Villa"));
        _mockPropertyRepository.Verify(r => r.CreateAsync(It.IsAny<Property>()), Times.Once);
    }

    [Test]
    public async Task CreatePropertyAsync_UnverifiedUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        _mockVerificationRepository.Setup(r => r.IsUserVerifiedAsync(ownerId)).ReturnsAsync(false);

        var propertyDto = new CreatePropertyDto { Title = "Luxury Villa" };

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _propertyService.CreatePropertyAsync(ownerId, propertyDto));
        Assert.That(ex.Message, Is.EqualTo("User must be verified to post a property."));
    }

    [Test]
    public async Task CreateLeaseProposalAsync_VerifiedUser_Succeeds()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _mockVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);

        var proposalDto = new CreateLeaseProposalDto
        {
            PropertyId = 1,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2027, 6, 1),
            MonthlyRent = 1200,
            UpfrontPayment = 1200,
            SecurityDeposit = 1200
        };

        // Act
        var result = await _leaseProposalService.CreateLeaseProposalAsync(tenantId, proposalDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PropertyId, Is.EqualTo(1));
        _mockLeaseProposalRepository.Verify(r => r.CreateAsync(It.IsAny<LeaseProposal>()), Times.Once);
    }

    [Test]
    public async Task CreateLeaseProposalAsync_UnverifiedUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        _mockVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(false);

        var proposalDto = new CreateLeaseProposalDto { PropertyId = 1 };

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.CreateLeaseProposalAsync(tenantId, proposalDto));
        Assert.That(ex.Message, Is.EqualTo("User must be verified to rent a property."));
    }
}
