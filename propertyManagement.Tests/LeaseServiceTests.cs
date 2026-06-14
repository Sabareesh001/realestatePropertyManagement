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
/// Unit tests for the <see cref="LeaseService"/> class.
/// </summary>
[TestFixture]
public class LeaseServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILeaseRepository> _mockLeaseRepository;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IUserVerificationRepository> _mockUserVerificationRepository;
    private Mock<ILeaseProposalRepository> _mockLeaseProposalRepository;
    private LeaseService _leaseService;

    /// <summary>
    /// Sets up repository mocks and initializes the service under test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLeaseRepository = new Mock<ILeaseRepository>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        _mockLeaseProposalRepository = new Mock<ILeaseProposalRepository>();

        _mockUnitOfWork.Setup(u => u.Leases).Returns(_mockLeaseRepository.Object);
        _mockUnitOfWork.Setup(u => u.Properties).Returns(_mockPropertyRepository.Object);
        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(u => u.UserVerifications).Returns(_mockUserVerificationRepository.Object);
        _mockUnitOfWork.Setup(u => u.LeaseProposals).Returns(_mockLeaseProposalRepository.Object);

        _leaseService = new LeaseService(_mockUnitOfWork.Object);
    }

    /// <summary>
    /// Verifies that CreateLeaseAsync succeeds when property owner is validated and tenant is verified.
    /// </summary>
    [Test]
    public async Task CreateLeaseAsync_ValidInputs_Succeeds()
    {
        var ownerId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var propertyId = 1;

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);

        var dto = new CreateLeaseDto
        {
            PropertyId = propertyId,
            TenantId = tenantId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500,
            StatusId = LeaseStatus.Draft
        };

        var result = await _leaseService.CreateLeaseAsync(ownerId, dto);

        Assert.That(result, Is.Not.Null);
        _mockLeaseRepository.Verify(r => r.CreateAsync(It.IsAny<Lease>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that CreateLeaseAsync throws an exception if the tenant is the owner of the property.
    /// </summary>
    [Test]
    public void CreateLeaseAsync_TenantIsOwner_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var propertyId = 1;

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });

        var dto = new CreateLeaseDto
        {
            PropertyId = propertyId,
            TenantId = ownerId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500,
            StatusId = LeaseStatus.Draft
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseService.CreateLeaseAsync(ownerId, dto));
        Assert.That(ex.Message, Is.EqualTo("Owner cannot lease their own property."));
    }

    /// <summary>
    /// Verifies that template verification approvals transition status to PendingSignature.
    /// </summary>
    [Test]
    public async Task VerifyLeaseTemplateAsync_Approve_TransitionsToPendingSignature()
    {
        var adminId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId, StatusId = LeaseStatus.Submitted };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var result = await _leaseService.VerifyLeaseTemplateAsync(adminId, leaseId, approve: true);

        Assert.That(result.StatusId, Is.EqualTo(LeaseStatus.PendingSignature));
        _mockLeaseRepository.Verify(r => r.UpdateAsync(lease), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that signing the lease transitions it to TenantSigned and attaches the document link.
    /// </summary>
    [Test]
    public async Task SignLeaseAsync_ValidSignature_TransitionsToTenantSigned()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId, TenantId = tenantId, StatusId = LeaseStatus.PendingSignature };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var dto = new SignLeaseDto { SignedAgreementDocumentUrl = "https://example.com/signed.pdf" };

        var result = await _leaseService.SignLeaseAsync(tenantId, leaseId, dto);

        Assert.That(result.StatusId, Is.EqualTo(LeaseStatus.TenantSigned));
        Assert.That(lease.SignedAgreementDocument, Is.Not.Null);
        Assert.That(lease.SignedAgreementDocument.DocumentUrl, Is.EqualTo(dto.SignedAgreementDocumentUrl));
        _mockLeaseRepository.Verify(r => r.UpdateAsync(lease), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that verifying the signed document transitions lease to Active and property to Occupied.
    /// </summary>
    [Test]
    public async Task VerifySignedLeaseAsync_Approve_ActivatesLeaseAndOccupiesProperty()
    {
        var adminId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var propertyId = 10;
        var lease = new Lease { Id = leaseId, PropertyId = propertyId, StatusId = LeaseStatus.TenantSigned };
        var property = new Property { Id = propertyId, AvailabilityStatusId = PropertyAvailabilityStatus.Available };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(property);

        var result = await _leaseService.VerifySignedLeaseAsync(adminId, leaseId, approve: true);

        Assert.That(result.StatusId, Is.EqualTo(LeaseStatus.Active));
        Assert.That(property.AvailabilityStatusId, Is.EqualTo(PropertyAvailabilityStatus.Occupied));
        _mockPropertyRepository.Verify(r => r.UpdateAsync(property), Times.Once);
        _mockLeaseRepository.Verify(r => r.UpdateAsync(lease), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
