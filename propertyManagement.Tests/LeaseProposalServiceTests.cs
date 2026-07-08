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
/// Unit tests for the <see cref="LeaseProposalService"/> class.
/// </summary>
[TestFixture]
public class LeaseProposalServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILeaseProposalRepository> _mockLeaseProposalRepository;
    private Mock<IUserVerificationRepository> _mockUserVerificationRepository;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IPropertyRepository> _mockPropertyRepository;
    private Mock<INotificationService> _mockNotificationService;
    private LeaseProposalService _leaseProposalService;

    /// <summary>
    /// Sets up repository mocks and initializes the service under test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLeaseProposalRepository = new Mock<ILeaseProposalRepository>();
        _mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPropertyRepository = new Mock<IPropertyRepository>();
        _mockNotificationService = new Mock<INotificationService>();

        _mockUnitOfWork.Setup(u => u.LeaseProposals).Returns(_mockLeaseProposalRepository.Object);
        _mockUnitOfWork.Setup(u => u.UserVerifications).Returns(_mockUserVerificationRepository.Object);
        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(u => u.Properties).Returns(_mockPropertyRepository.Object);

        _leaseProposalService = new LeaseProposalService(_mockUnitOfWork.Object, _mockNotificationService.Object);
    }

    /// <summary>
    /// Verifies that verified tenants can submit rent requests successfully.
    /// </summary>
    [Test]
    public async Task CreateLeaseProposalAsync_VerifiedTenant_Succeeds()
    {
        var tenantId = Guid.NewGuid();
        var propertyId = 1;

        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property
        {
            Id = propertyId,
            VerificationStatusId = PropertyVerificationStatus.Verified,
            AvailabilityStatusId = PropertyAvailabilityStatus.Available
        });
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId, FirstName = "John", LastName = "Doe" });

        var dto = new CreateLeaseProposalDto
        {
            PropertyId = propertyId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            MonthlyRent = 1000
        };

        var result = await _leaseProposalService.CreateLeaseProposalAsync(tenantId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusId, Is.EqualTo(ProposalStatus.Draft));
        _mockLeaseProposalRepository.Verify(r => r.CreateAsync(It.IsAny<LeaseProposal>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that unverified tenants are blocked from submitting rent requests.
    /// </summary>
    [Test]
    public void CreateLeaseProposalAsync_UnverifiedTenant_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(false);

        var dto = new CreateLeaseProposalDto { PropertyId = 1 };

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.CreateLeaseProposalAsync(tenantId, dto));
    }

    /// <summary>
    /// Verifies that property owners are blocked from submitting rent requests on their own properties.
    /// </summary>
    [Test]
    public void CreateLeaseProposalAsync_TenantIsOwner_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var propertyId = 1;

        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = tenantId });

        var dto = new CreateLeaseProposalDto { PropertyId = propertyId };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.CreateLeaseProposalAsync(tenantId, dto));
        Assert.That(ex.Message, Is.EqualTo("Owner cannot lease their own property."));
    }

    /// <summary>
    /// Verifies that creating a lease proposal for an unverified property throws an InvalidOperationException.
    /// </summary>
    [Test]
    public void CreateLeaseProposalAsync_UnverifiedProperty_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var propertyId = 1;

        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property
        {
            Id = propertyId,
            OwnerId = Guid.NewGuid(),
            VerificationStatusId = PropertyVerificationStatus.Submitted,
            AvailabilityStatusId = PropertyAvailabilityStatus.Available
        });

        var dto = new CreateLeaseProposalDto { PropertyId = propertyId };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.CreateLeaseProposalAsync(tenantId, dto));
        Assert.That(ex.Message, Is.EqualTo("Property must be verified before a lease proposal can be created."));
    }

    /// <summary>
    /// Verifies that creating a lease proposal for an unavailable property throws an InvalidOperationException.
    /// </summary>
    [Test]
    public void CreateLeaseProposalAsync_UnavailableProperty_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var propertyId = 1;

        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property
        {
            Id = propertyId,
            OwnerId = Guid.NewGuid(),
            VerificationStatusId = PropertyVerificationStatus.Verified,
            AvailabilityStatusId = PropertyAvailabilityStatus.Occupied
        });

        var dto = new CreateLeaseProposalDto { PropertyId = propertyId };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.CreateLeaseProposalAsync(tenantId, dto));
        Assert.That(ex.Message, Is.EqualTo("Property is not available for lease."));
    }

    /// <summary>
    /// Verifies that a verified tenant can successfully submit a draft lease proposal.
    /// </summary>
    [Test]
    public async Task SubmitLeaseProposalAsync_ValidDraft_Succeeds()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            StatusId = ProposalStatus.Draft
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });

        var result = await _leaseProposalService.SubmitLeaseProposalAsync(tenantId, proposalId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusId, Is.EqualTo(ProposalStatus.Submitted));
        _mockLeaseProposalRepository.Verify(r => r.UpdateAsync(proposal), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that submitting a non-existent proposal throws a KeyNotFoundException.
    /// </summary>
    [Test]
    public void SubmitLeaseProposalAsync_NonExistentProposal_ThrowsKeyNotFoundException()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync((LeaseProposal?)null);

        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _leaseProposalService.SubmitLeaseProposalAsync(tenantId, proposalId));
    }

    /// <summary>
    /// Verifies that submitting another tenant's proposal throws an UnauthorizedAccessException.
    /// </summary>
    [Test]
    public void SubmitLeaseProposalAsync_UnauthorizedTenant_ThrowsUnauthorizedAccessException()
    {
        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = otherTenantId,
            StatusId = ProposalStatus.Draft
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _leaseProposalService.SubmitLeaseProposalAsync(tenantId, proposalId));
    }

    /// <summary>
    /// Verifies that submitting a proposal that is not in Draft status throws an InvalidOperationException.
    /// </summary>
    [Test]
    public void SubmitLeaseProposalAsync_NonDraftProposal_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            StatusId = ProposalStatus.Submitted
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.SubmitLeaseProposalAsync(tenantId, proposalId));
        Assert.That(ex.Message, Is.EqualTo("Only draft proposals can be submitted."));
    }

    /// <summary>
    /// Verifies that submitting a draft proposal when tenant is unverified throws an InvalidOperationException.
    /// </summary>
    [Test]
    public void SubmitLeaseProposalAsync_UnverifiedTenant_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            StatusId = ProposalStatus.Draft
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(false);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.SubmitLeaseProposalAsync(tenantId, proposalId));
        Assert.That(ex.Message, Is.EqualTo("User must be verified to rent a property."));
    }

    /// <summary>
    /// Verifies that the property owner can successfully accept a submitted proposal.
    /// </summary>
    [Test]
    public async Task ReviewProposalAsync_ValidAccept_Succeeds()
    {
        var ownerId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var propertyId = 10;

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            PropertyId = propertyId,
            StatusId = ProposalStatus.Submitted
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });

        var result = await _leaseProposalService.ReviewProposalAsync(ownerId, proposalId, accept: true);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusId, Is.EqualTo(ProposalStatus.Approved));
        Assert.That(proposal.ReviewedBy, Is.EqualTo(ownerId));
        _mockLeaseProposalRepository.Verify(r => r.UpdateAsync(proposal), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that the property owner can successfully reject a submitted proposal.
    /// </summary>
    [Test]
    public async Task ReviewProposalAsync_ValidReject_Succeeds()
    {
        var ownerId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var propertyId = 10;

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            PropertyId = propertyId,
            StatusId = ProposalStatus.Submitted
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });

        var result = await _leaseProposalService.ReviewProposalAsync(ownerId, proposalId, accept: false);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusId, Is.EqualTo(ProposalStatus.Rejected));
        Assert.That(proposal.ReviewedBy, Is.EqualTo(ownerId));
        _mockLeaseProposalRepository.Verify(r => r.UpdateAsync(proposal), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that reviewing a non-existent proposal throws a KeyNotFoundException.
    /// </summary>
    [Test]
    public void ReviewProposalAsync_NonExistentProposal_ThrowsKeyNotFoundException()
    {
        var ownerId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync((LeaseProposal?)null);

        Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _leaseProposalService.ReviewProposalAsync(ownerId, proposalId, accept: true));
    }

    /// <summary>
    /// Verifies that reviewing a proposal when caller is not the property owner throws an UnauthorizedAccessException.
    /// </summary>
    [Test]
    public void ReviewProposalAsync_UnauthorizedOwner_ThrowsUnauthorizedAccessException()
    {
        var ownerId = Guid.NewGuid();
        var otherOwnerId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var propertyId = 10;

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            PropertyId = propertyId,
            StatusId = ProposalStatus.Submitted
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = otherOwnerId });

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _leaseProposalService.ReviewProposalAsync(ownerId, proposalId, accept: true));
    }

    /// <summary>
    /// Verifies that reviewing a proposal not in Submitted status throws an InvalidOperationException.
    /// </summary>
    [Test]
    public void ReviewProposalAsync_NonSubmittedProposal_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var propertyId = 10;

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            PropertyId = propertyId,
            StatusId = ProposalStatus.Draft
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.ReviewProposalAsync(ownerId, proposalId, accept: true));
        Assert.That(ex.Message, Is.EqualTo("Only submitted proposals can be reviewed."));
    }

    /// <summary>
    /// Verifies that a tenant can cancel a proposal in Draft status.
    /// </summary>
    [Test]
    public async Task CancelProposalAsync_DraftStatus_Succeeds()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            StatusId = ProposalStatus.Draft
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });

        var result = await _leaseProposalService.CancelProposalAsync(tenantId, proposalId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusId, Is.EqualTo(ProposalStatus.Cancelled));
        _mockLeaseProposalRepository.Verify(r => r.UpdateAsync(proposal), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that a tenant can cancel a proposal in Submitted status.
    /// </summary>
    [Test]
    public async Task CancelProposalAsync_SubmittedStatus_Succeeds()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            StatusId = ProposalStatus.Submitted
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });

        var result = await _leaseProposalService.CancelProposalAsync(tenantId, proposalId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusId, Is.EqualTo(ProposalStatus.Cancelled));
        _mockLeaseProposalRepository.Verify(r => r.UpdateAsync(proposal), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that cancelling a proposal created by another tenant throws an UnauthorizedAccessException.
    /// </summary>
    [Test]
    public void CancelProposalAsync_UnauthorizedTenant_ThrowsUnauthorizedAccessException()
    {
        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = otherTenantId,
            StatusId = ProposalStatus.Draft
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _leaseProposalService.CancelProposalAsync(tenantId, proposalId));
    }

    /// <summary>
    /// Verifies that cancelling a proposal not in Draft or Submitted status throws an InvalidOperationException.
    /// </summary>
    [Test]
    public void CancelProposalAsync_NonCancellableStatus_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        var proposal = new LeaseProposal
        {
            Id = proposalId,
            TenantId = tenantId,
            StatusId = ProposalStatus.Approved
        };

        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(proposal);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.CancelProposalAsync(tenantId, proposalId));
        Assert.That(ex.Message, Is.EqualTo("Only draft or submitted proposals can be cancelled."));
    }

    /// <summary>
    /// Verifies that CreateLeaseProposalAsync throws an exception if there is an overlapping lease proposal.
    /// </summary>
    [Test]
    public void CreateLeaseProposalAsync_OverlappingDates_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var propertyId = 1;
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(6));

        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property
        {
            Id = propertyId,
            OwnerId = Guid.NewGuid(),
            VerificationStatusId = PropertyVerificationStatus.Verified,
            AvailabilityStatusId = PropertyAvailabilityStatus.Available
        });

        // Mock overlap query to return true
        _mockLeaseProposalRepository.Setup(r => r.HasOverlappingProposalAsync(propertyId, startDate, endDate)).ReturnsAsync(true);

        var dto = new CreateLeaseProposalDto
        {
            PropertyId = propertyId,
            StartDate = startDate,
            EndDate = endDate
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseProposalService.CreateLeaseProposalAsync(tenantId, dto));
        Assert.That(ex.Message, Is.EqualTo("A lease proposal already exists for this property during the specified time period."));
    }
}
