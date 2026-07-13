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
    private Mock<IDocumentRepository> _mockDocumentRepository;
    private Mock<INotificationService> _mockNotificationService;
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
        _mockDocumentRepository = new Mock<IDocumentRepository>();
        _mockNotificationService = new Mock<INotificationService>();

        _mockUnitOfWork.Setup(u => u.Leases).Returns(_mockLeaseRepository.Object);
        _mockUnitOfWork.Setup(u => u.Properties).Returns(_mockPropertyRepository.Object);
        _mockUnitOfWork.Setup(u => u.Users).Returns(_mockUserRepository.Object);
        _mockUnitOfWork.Setup(u => u.UserVerifications).Returns(_mockUserVerificationRepository.Object);
        _mockUnitOfWork.Setup(u => u.LeaseProposals).Returns(_mockLeaseProposalRepository.Object);
        _mockUnitOfWork.Setup(u => u.Documents).Returns(_mockDocumentRepository.Object);

        _mockDocumentRepository.Setup(r => r.CreateAsync(It.IsAny<Document>()))
            .ReturnsAsync((Document doc) => doc);

        _mockUserRepository.Setup(r => r.GetUserIdsByRoleAsync(It.IsAny<int>()))
            .ReturnsAsync(Array.Empty<Guid>());

        _leaseService = new LeaseService(_mockUnitOfWork.Object, _mockNotificationService.Object);
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
        var proposalId = Guid.NewGuid();

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(new LeaseProposal { Id = proposalId, PropertyId = propertyId, TenantId = tenantId });

        var dto = new CreateLeaseDto
        {
            PropertyId = propertyId,
            TenantId = tenantId,
            ProposalId = proposalId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500
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
            ProposalId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500
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

    /// <summary>
    /// Verifies that CreateLeaseAsync throws an exception if the lease proposal does not exist.
    /// </summary>
    [Test]
    public void CreateLeaseAsync_ProposalNotFound_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var propertyId = 1;
        var proposalId = Guid.NewGuid();

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync((LeaseProposal?)null);

        var dto = new CreateLeaseDto
        {
            PropertyId = propertyId,
            TenantId = tenantId,
            ProposalId = proposalId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseService.CreateLeaseAsync(ownerId, dto));
        Assert.That(ex.Message, Is.EqualTo("Lease proposal not found."));
    }

    /// <summary>
    /// Verifies that CreateLeaseAsync throws an exception if the lease proposal does not match the property or tenant.
    /// </summary>
    [Test]
    public void CreateLeaseAsync_ProposalMismatch_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var propertyId = 1;
        var proposalId = Guid.NewGuid();

        _mockPropertyRepository.Setup(r => r.GetByIdAsync(propertyId)).ReturnsAsync(new Property { Id = propertyId, OwnerId = ownerId });
        _mockUserRepository.Setup(r => r.GetByIdAsync(tenantId)).ReturnsAsync(new User { Id = tenantId });
        _mockUserVerificationRepository.Setup(r => r.IsUserVerifiedAsync(tenantId)).ReturnsAsync(true);
        _mockLeaseProposalRepository.Setup(r => r.GetByIdAsync(proposalId)).ReturnsAsync(new LeaseProposal { Id = proposalId, PropertyId = 999, TenantId = tenantId });

        var dto = new CreateLeaseDto
        {
            PropertyId = propertyId,
            TenantId = tenantId,
            ProposalId = proposalId,
            StartDate = DateOnly.FromDateTime(DateTime.Today),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(12)),
            MonthlyRent = 1500,
            UpfrontPayment = 3000,
            SecurityDeposit = 1500
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseService.CreateLeaseAsync(ownerId, dto));
        Assert.That(ex.Message, Is.EqualTo("Lease proposal does not match the specified property or tenant."));
    }

    /// <summary>
    /// Verifies that UpdateLeaseAsync replaces existing lease documents with new ones if provided.
    /// </summary>
    [Test]
    public async Task UpdateLeaseAsync_WithDocuments_ReplacesDocuments()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var propertyId = 1;
        var existingLease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Draft,
            PropertyNavigation = new Property { Id = propertyId, OwnerId = ownerId },
            Documents = new List<Document>
            {
                new Document { Id = Guid.NewGuid(), DocumentTypeId = 1, DocumentUrl = "http://old.com" }
            }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(existingLease);

        var dto = new UpdateLeaseDto
        {
            Documents = new List<LeaseDocumentDto>
            {
                new LeaseDocumentDto { DocumentTypeId = 2, DocumentNumber = "DOC-002", DocumentUrl = "http://new.com" }
            }
        };

        var result = await _leaseService.UpdateLeaseAsync(ownerId, leaseId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(existingLease.Documents.Count, Is.EqualTo(1));
        Assert.That(existingLease.Documents.First().DocumentUrl, Is.EqualTo("http://new.com"));
        _mockLeaseRepository.Verify(r => r.UpdateAsync(existingLease), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that GetLeaseDocumentsAsync returns document response DTOs for authorized users.
    /// </summary>
    [Test]
    public async Task GetLeaseDocumentsAsync_AuthorizedUser_ReturnsDocuments()
    {
        var leaseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            TenantId = userId,
            StatusId = LeaseStatus.Active,
            Documents = new List<Document>
            {
                new Document { Id = Guid.NewGuid(), DocumentTypeId = 1, DocumentUrl = "http://doc.com" }
            }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);

        var result = await _leaseService.GetLeaseDocumentsAsync(leaseId, userId, new[] { "Tenant" }, new PaginationParams());

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items.Count(), Is.EqualTo(1));
        Assert.That(result.Items.First().DocumentUrl, Is.EqualTo("http://doc.com"));
    }

    /// <summary>
    /// Verifies that GetLeaseDocumentsAsync throws an exception when the tenant is unauthorized (e.g. lease is in Draft state).
    /// </summary>
    [Test]
    public void GetLeaseDocumentsAsync_TenantDraftLease_ThrowsUnauthorizedAccessException()
    {
        var leaseId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            TenantId = tenantId,
            StatusId = LeaseStatus.Draft,
            Documents = new List<Document>()
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);

        Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _leaseService.GetLeaseDocumentsAsync(leaseId, tenantId, new[] { "Tenant" }, new PaginationParams()));
    }

    /// <summary>
    /// Verifies that GetMyLeasesAsync combines and returns distinct leases matching any of the user's multiple roles.
    /// </summary>
    [Test]
    public async Task GetMyLeasesAsync_MultipleRoles_ReturnsCombinedDistinctLeases()
    {
        var userId = Guid.NewGuid();
        var lease1 = new Lease
        {
            Id = Guid.NewGuid(),
            PropertyNavigation = new Property { OwnerId = userId },
            StatusId = LeaseStatus.Active
        };
        var lease2 = new Lease
        {
            Id = Guid.NewGuid(),
            TenantId = userId,
            StatusId = LeaseStatus.Active
        };
        var lease3 = new Lease
        {
            Id = Guid.NewGuid(),
            TenantId = userId,
            StatusId = LeaseStatus.Draft // Tenant cannot see draft
        };

        var allLeases = new List<Lease> { lease1, lease2, lease3 };
        _mockLeaseRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(allLeases);

        var roles = new[] { "Owner", "Tenant" };
        var result = await _leaseService.GetMyLeasesAsync(userId, roles, new PaginationParams());

        Assert.That(result, Is.Not.Null);
        var resultList = result.Items.ToList();
        Assert.That(resultList.Count, Is.EqualTo(2));
        Assert.That(resultList.Any(l => l.Id == lease1.Id), Is.True);
        Assert.That(resultList.Any(l => l.Id == lease2.Id), Is.True);
        Assert.That(resultList.Any(l => l.Id == lease3.Id), Is.False);
    }

    /// <summary>
    /// Verifies that GetLeaseDocumentsAsync deduplicates documents by their unique identifier.
    /// </summary>
    [Test]
    public async Task GetLeaseDocumentsAsync_DuplicateDocuments_ReturnsDistinctList()
    {
        var leaseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var docId = Guid.NewGuid();
        var doc = new Document { Id = docId, DocumentTypeId = 1, DocumentUrl = "http://doc.com" };

        var lease = new Lease
        {
            Id = leaseId,
            TenantId = userId,
            StatusId = LeaseStatus.Active,
            AgreementDocument = doc,
            Documents = new List<Document> { doc }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);

        var result = await _leaseService.GetLeaseDocumentsAsync(leaseId, userId, new[] { "Tenant" }, new PaginationParams());

        Assert.That(result, Is.Not.Null);
        var list = result.Items.ToList();
        Assert.That(list.Count, Is.EqualTo(1));
        Assert.That(list[0].Id, Is.EqualTo(docId));
    }

    /// <summary>
    /// Verifies that GetLeaseDocumentsAsync filters out soft-deleted documents.
    /// </summary>
    [Test]
    public async Task GetLeaseDocumentsAsync_SoftDeletedDocuments_FiltersThemOut()
    {
        var leaseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var activeDocId = Guid.NewGuid();
        var softDeletedDocId1 = Guid.NewGuid();
        var softDeletedDocId2 = Guid.NewGuid();
        var softDeletedDocId3 = Guid.NewGuid();

        var lease = new Lease
        {
            Id = leaseId,
            TenantId = userId,
            StatusId = LeaseStatus.Active,
            AgreementDocument = new Document { Id = softDeletedDocId1, DocumentTypeId = 1, DocumentUrl = "http://soft1.com", DeletedAt = DateTime.UtcNow },
            SignedAgreementDocument = new Document { Id = softDeletedDocId2, DocumentTypeId = 2, DocumentUrl = "http://soft2.com", DeletedAt = DateTime.UtcNow },
            Documents = new List<Document>
            {
                new Document { Id = softDeletedDocId3, DocumentTypeId = 3, DocumentUrl = "http://soft3.com", DeletedAt = DateTime.UtcNow },
                new Document { Id = activeDocId, DocumentTypeId = 4, DocumentUrl = "http://active.com", DeletedAt = null }
            }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);

        var result = await _leaseService.GetLeaseDocumentsAsync(leaseId, userId, new[] { "Tenant" }, new PaginationParams());

        Assert.That(result, Is.Not.Null);
        var list = result.Items.ToList();
        Assert.That(list.Count, Is.EqualTo(1));
        Assert.That(list[0].Id, Is.EqualTo(activeDocId));
    }

    /// <summary>
    /// Verifies that GetLeaseByIdAsync filters out URLs of soft-deleted agreement documents.
    /// </summary>
    [Test]
    public async Task GetLeaseByIdAsync_SoftDeletedCoreDocuments_ReturnsNullUrls()
    {
        var leaseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            TenantId = userId,
            StatusId = LeaseStatus.Active,
            AgreementDocument = new Document { Id = Guid.NewGuid(), DocumentTypeId = 4, DocumentUrl = "http://soft1.com", DeletedAt = DateTime.UtcNow },
            SignedAgreementDocument = new Document { Id = Guid.NewGuid(), DocumentTypeId = 5, DocumentUrl = "http://soft2.com", DeletedAt = DateTime.UtcNow }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var result = await _leaseService.GetLeaseByIdAsync(leaseId, userId, new[] { "Tenant" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.AgreementDocumentUrl, Is.Null);
        Assert.That(result.SignedAgreementDocumentUrl, Is.Null);
    }

    /// <summary>
    /// Verifies that SubmitLeaseAsync transitions draft lease with an agreement document to Submitted status.
    /// </summary>
    [Test]
    public async Task SubmitLeaseAsync_ValidDraftWithAgreement_TransitionsToSubmitted()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var propertyId = 1;
        var lease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Draft,
            PropertyNavigation = new Property { Id = propertyId, OwnerId = ownerId },
            AgreementDocument = new Document { Id = Guid.NewGuid(), DocumentTypeId = 4, DocumentUrl = "http://agreement.com" }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);
        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var result = await _leaseService.SubmitLeaseAsync(ownerId, leaseId);

        Assert.That(result, Is.Not.Null);
        Assert.That(lease.StatusId, Is.EqualTo(LeaseStatus.Submitted));
        _mockLeaseRepository.Verify(r => r.UpdateAsync(lease), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that SubmitLeaseAsync throws InvalidOperationException if the agreement document is missing.
    /// </summary>
    [Test]
    public void SubmitLeaseAsync_MissingAgreementDocument_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var propertyId = 1;
        var lease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Draft,
            PropertyNavigation = new Property { Id = propertyId, OwnerId = ownerId },
            AgreementDocument = null
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseService.SubmitLeaseAsync(ownerId, leaseId));

        Assert.That(ex.Message, Is.EqualTo("Please upload an agreement document before submitting the lease."));
    }

    /// <summary>
    /// Verifies that SubmitLeaseAsync throws InvalidOperationException if the lease is not in Draft status.
    /// </summary>
    [Test]
    public void SubmitLeaseAsync_NonDraftStatus_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var propertyId = 1;
        var lease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Submitted,
            PropertyNavigation = new Property { Id = propertyId, OwnerId = ownerId },
            AgreementDocument = new Document { Id = Guid.NewGuid(), DocumentTypeId = 4, DocumentUrl = "http://agreement.com" }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseService.SubmitLeaseAsync(ownerId, leaseId));

        Assert.That(ex.Message, Is.EqualTo("Only leases in Draft status can be submitted."));
    }

    /// <summary>
    /// Verifies that UpdateLeaseAsync throws InvalidOperationException if StatusId is updated directly to Submitted.
    /// </summary>
    [Test]
    public void UpdateLeaseAsync_AttemptToDirectlySubmit_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var propertyId = 1;
        var lease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Draft,
            PropertyNavigation = new Property { Id = propertyId, OwnerId = ownerId }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdWithDocumentsAsync(leaseId)).ReturnsAsync(lease);

        var dto = new UpdateLeaseDto
        {
            StatusId = LeaseStatus.Submitted
        };

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _leaseService.UpdateLeaseAsync(ownerId, leaseId, dto));

        Assert.That(ex.Message, Is.EqualTo("Please use the submit endpoint to submit the lease."));
    }
}
