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
/// Unit tests for <see cref="ComplaintService"/> covering creation, status transitions, and comment gating.
/// </summary>
[TestFixture]
public class ComplaintServiceTests
{
    private Mock<IUnitOfWork> _mockUow;
    private Mock<ILeaseRepository> _mockLeaseRepo;
    private Mock<IComplaintRepository> _mockComplaintRepo;
    private Mock<IComplaintCommentRepository> _mockCommentRepo;
    private Mock<IUserRepository> _mockUserRepo;
    private ComplaintService _service;

    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly Guid LeaseId = Guid.NewGuid();
    private static readonly int PropertyId = 42;

    /// <summary>Sets up mocks and initializes the service.</summary>
    [SetUp]
    public void SetUp()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockLeaseRepo = new Mock<ILeaseRepository>();
        _mockComplaintRepo = new Mock<IComplaintRepository>();
        _mockCommentRepo = new Mock<IComplaintCommentRepository>();
        _mockUserRepo = new Mock<IUserRepository>();

        _mockUow.Setup(u => u.Leases).Returns(_mockLeaseRepo.Object);
        _mockUow.Setup(u => u.Complaints).Returns(_mockComplaintRepo.Object);
        _mockUow.Setup(u => u.ComplaintComments).Returns(_mockCommentRepo.Object);
        _mockUow.Setup(u => u.Users).Returns(_mockUserRepo.Object);
        _mockUow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ComplaintService(_mockUow.Object);
    }

    // ── CreateComplaintAsync ─────────────────────────────────────────────────

    /// <summary>
    /// CreateComplaintAsync succeeds when caller is the lease's tenant and the lease is Active.
    /// </summary>
    [Test]
    public async Task CreateComplaintAsync_ValidTenantActiveLease_Succeeds()
    {
        var lease = ActiveLease();
        var complaint = MakeComplaint(ComplaintStatus.Open);
        complaint.Id = Guid.NewGuid();

        _mockLeaseRepo.Setup(r => r.GetByIdAsync(LeaseId)).ReturnsAsync(lease);
        _mockComplaintRepo.Setup(r => r.CreateAsync(It.IsAny<Complaint>())).ReturnsAsync((Complaint c) => c);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(complaint);

        var result = await _service.CreateComplaintAsync(TenantId, ValidCreateDto());

        Assert.That(result, Is.Not.Null);
        _mockComplaintRepo.Verify(r => r.CreateAsync(It.Is<Complaint>(c =>
            c.TenantId == TenantId &&
            c.StatusId == ComplaintStatus.Open &&
            c.CreatedBy == TenantId)), Times.Once);
    }

    /// <summary>
    /// CreateComplaintAsync throws UnauthorizedAccessException when caller is not the lease tenant.
    /// </summary>
    [Test]
    public void CreateComplaintAsync_WrongTenant_ThrowsUnauthorized()
    {
        var lease = ActiveLease();
        lease.TenantId = Guid.NewGuid(); // different tenant

        _mockLeaseRepo.Setup(r => r.GetByIdAsync(LeaseId)).ReturnsAsync(lease);

        Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.CreateComplaintAsync(TenantId, ValidCreateDto()));
    }

    /// <summary>
    /// CreateComplaintAsync throws InvalidOperationException when the lease is not Active.
    /// </summary>
    [Test]
    public void CreateComplaintAsync_InactiveLease_ThrowsInvalidOperation()
    {
        var lease = ActiveLease();
        lease.StatusId = LeaseStatus.PendingSignature;

        _mockLeaseRepo.Setup(r => r.GetByIdAsync(LeaseId)).ReturnsAsync(lease);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateComplaintAsync(TenantId, ValidCreateDto()));
    }

    /// <summary>
    /// CreateComplaintAsync throws KeyNotFoundException when the lease does not exist.
    /// </summary>
    [Test]
    public void CreateComplaintAsync_LeaseNotFound_ThrowsKeyNotFound()
    {
        _mockLeaseRepo.Setup(r => r.GetByIdAsync(LeaseId)).ReturnsAsync((Lease?)null);

        Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CreateComplaintAsync(TenantId, ValidCreateDto()));
    }

    // ── GetComplaintByIdAsync access control ─────────────────────────────────

    /// <summary>
    /// GetComplaintByIdAsync throws UnauthorizedAccessException for a non-participant.
    /// </summary>
    [Test]
    public void GetComplaintByIdAsync_NonParticipant_ThrowsUnauthorized()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        var strangerRoles = new List<string> { "Tenant" };
        var strangerId = Guid.NewGuid(); // not TenantId or OwnerId

        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.GetComplaintByIdAsync(complaint.Id, strangerId, strangerRoles));
    }

    /// <summary>
    /// GetComplaintByIdAsync succeeds for the tenant creator.
    /// </summary>
    [Test]
    public async Task GetComplaintByIdAsync_TenantCreator_Succeeds()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        var result = await _service.GetComplaintByIdAsync(complaint.Id, TenantId, new[] { "Tenant" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(complaint.Id));
    }

    /// <summary>
    /// GetComplaintByIdAsync succeeds for the property owner.
    /// </summary>
    [Test]
    public async Task GetComplaintByIdAsync_Owner_Succeeds()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        var result = await _service.GetComplaintByIdAsync(complaint.Id, OwnerId, new[] { "Owner" });

        Assert.That(result, Is.Not.Null);
    }

    // ── UpdateStatusAsync — valid transitions ────────────────────────────────

    [TestCase(ComplaintStatus.Open, ComplaintStatus.InProgress, "Owner")]
    [TestCase(ComplaintStatus.Open, ComplaintStatus.Resolved, "Owner")]
    [TestCase(ComplaintStatus.InProgress, ComplaintStatus.Resolved, "Owner")]
    [TestCase(ComplaintStatus.Resolved, ComplaintStatus.InProgress, "Owner")]
    /// <summary>
    /// Owner-initiated valid transitions succeed.
    /// </summary>
    public async Task UpdateStatusAsync_OwnerValidTransition_Succeeds(int from, int to, string role)
    {
        var complaint = MakeComplaint(from);
        var updated = MakeComplaint(to);

        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);
        _mockComplaintRepo.Setup(r => r.UpdateAsync(It.IsAny<Complaint>())).Returns(Task.CompletedTask);
        _mockComplaintRepo.SetupSequence(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(complaint)
            .ReturnsAsync(updated);

        var result = await _service.UpdateStatusAsync(
            complaint.Id, OwnerId, new[] { "Owner" },
            new UpdateComplaintStatusDto { StatusId = to });

        Assert.That(result, Is.Not.Null);
    }

    /// <summary>
    /// Tenant-creator can cancel an open complaint.
    /// </summary>
    [Test]
    public async Task UpdateStatusAsync_TenantCancelsOpen_Succeeds()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        var cancelled = MakeComplaint(ComplaintStatus.Cancelled);

        _mockComplaintRepo.SetupSequence(r => r.GetByIdAsync(complaint.Id))
            .ReturnsAsync(complaint)
            .ReturnsAsync(cancelled);
        _mockComplaintRepo.Setup(r => r.UpdateAsync(It.IsAny<Complaint>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateStatusAsync(
            complaint.Id, TenantId, new[] { "Tenant" },
            new UpdateComplaintStatusDto { StatusId = ComplaintStatus.Cancelled });

        Assert.That(result, Is.Not.Null);
    }

    /// <summary>
    /// Tenant-creator can close a resolved complaint.
    /// </summary>
    [Test]
    public async Task UpdateStatusAsync_TenantClosesResolved_Succeeds()
    {
        var complaint = MakeComplaint(ComplaintStatus.Resolved);
        var closed = MakeComplaint(ComplaintStatus.Closed);

        _mockComplaintRepo.SetupSequence(r => r.GetByIdAsync(complaint.Id))
            .ReturnsAsync(complaint)
            .ReturnsAsync(closed);
        _mockComplaintRepo.Setup(r => r.UpdateAsync(It.IsAny<Complaint>())).Returns(Task.CompletedTask);

        var result = await _service.UpdateStatusAsync(
            complaint.Id, TenantId, new[] { "Tenant" },
            new UpdateComplaintStatusDto { StatusId = ComplaintStatus.Closed });

        Assert.That(result, Is.Not.Null);
    }

    // ── UpdateStatusAsync — invalid/wrong-role transitions ───────────────────

    /// <summary>
    /// Transitioning a Closed complaint throws InvalidOperationException (terminal status).
    /// </summary>
    [Test]
    public void UpdateStatusAsync_ClosedComplaint_ThrowsInvalidOperation()
    {
        var complaint = MakeComplaint(ComplaintStatus.Closed);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateStatusAsync(
                complaint.Id, OwnerId, new[] { "Owner" },
                new UpdateComplaintStatusDto { StatusId = ComplaintStatus.InProgress }));
    }

    /// <summary>
    /// Tenant trying to set Open→InProgress throws UnauthorizedAccessException (owner/admin only).
    /// </summary>
    [Test]
    public void UpdateStatusAsync_TenantTriesToStartProgress_ThrowsUnauthorized()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UpdateStatusAsync(
                complaint.Id, TenantId, new[] { "Tenant" },
                new UpdateComplaintStatusDto { StatusId = ComplaintStatus.InProgress }));
    }

    /// <summary>
    /// Owner trying to close a Resolved complaint (tenant-only) throws UnauthorizedAccessException.
    /// </summary>
    [Test]
    public void UpdateStatusAsync_OwnerTriesToClose_ThrowsUnauthorized()
    {
        var complaint = MakeComplaint(ComplaintStatus.Resolved);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.UpdateStatusAsync(
                complaint.Id, OwnerId, new[] { "Owner" },
                new UpdateComplaintStatusDto { StatusId = ComplaintStatus.Closed }));
    }

    /// <summary>
    /// Completely invalid transition (e.g. Open→Closed) throws InvalidOperationException.
    /// </summary>
    [Test]
    public void UpdateStatusAsync_InvalidTransition_ThrowsInvalidOperation()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateStatusAsync(
                complaint.Id, OwnerId, new[] { "Owner" },
                new UpdateComplaintStatusDto { StatusId = ComplaintStatus.Closed }));
    }

    // ── AddCommentAsync ──────────────────────────────────────────────────────

    /// <summary>
    /// AddCommentAsync is blocked on a Closed complaint.
    /// </summary>
    [Test]
    public void AddCommentAsync_ClosedComplaint_ThrowsInvalidOperation()
    {
        var complaint = MakeComplaint(ComplaintStatus.Closed);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddCommentAsync(
                complaint.Id, TenantId, new[] { "Tenant" },
                new AddCommentDto { Message = "Hello" }));
    }

    /// <summary>
    /// AddCommentAsync is blocked on a Cancelled complaint.
    /// </summary>
    [Test]
    public void AddCommentAsync_CancelledComplaint_ThrowsInvalidOperation()
    {
        var complaint = MakeComplaint(ComplaintStatus.Cancelled);
        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);

        Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddCommentAsync(
                complaint.Id, TenantId, new[] { "Tenant" },
                new AddCommentDto { Message = "Hello" }));
    }

    /// <summary>
    /// AddCommentAsync succeeds on an Open complaint for the tenant.
    /// </summary>
    [Test]
    public async Task AddCommentAsync_OpenComplaintTenant_Succeeds()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        var author = MakeTenantUser(TenantId);
        var comment = new ComplaintComment
        {
            Id = Guid.NewGuid(),
            ComplaintId = complaint.Id,
            AuthorId = TenantId,
            Message = "Hello",
            CreatedAt = DateTime.UtcNow
        };

        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);
        _mockCommentRepo.Setup(r => r.CreateAsync(It.IsAny<ComplaintComment>())).ReturnsAsync(comment);
        _mockUserRepo.Setup(r => r.GetByIdAsync(TenantId)).ReturnsAsync(author);

        var result = await _service.AddCommentAsync(
            complaint.Id, TenantId, new[] { "Tenant" },
            new AddCommentDto { Message = "Hello" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.AuthorRole, Is.EqualTo("Tenant"));
    }

    // ── authorRole computation ────────────────────────────────────────────────

    /// <summary>
    /// authorRole is "Owner" when the comment author is the property owner.
    /// </summary>
    [Test]
    public async Task AddCommentAsync_AuthorIsOwner_AuthorRoleIsOwner()
    {
        var complaint = MakeComplaint(ComplaintStatus.Open);
        var ownerUser = MakeOwnerUser(OwnerId);
        var comment = new ComplaintComment
        {
            Id = Guid.NewGuid(),
            ComplaintId = complaint.Id,
            AuthorId = OwnerId,
            Message = "Scheduling a repair",
            CreatedAt = DateTime.UtcNow
        };

        _mockComplaintRepo.Setup(r => r.GetByIdAsync(complaint.Id)).ReturnsAsync(complaint);
        _mockCommentRepo.Setup(r => r.CreateAsync(It.IsAny<ComplaintComment>())).ReturnsAsync(comment);
        _mockUserRepo.Setup(r => r.GetByIdAsync(OwnerId)).ReturnsAsync(ownerUser);

        var result = await _service.AddCommentAsync(
            complaint.Id, OwnerId, new[] { "Owner" },
            new AddCommentDto { Message = "Scheduling a repair" });

        Assert.That(result.AuthorRole, Is.EqualTo("Owner"));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static Lease ActiveLease() => new()
    {
        Id = LeaseId,
        TenantId = TenantId,
        StatusId = LeaseStatus.Active,
        PropertyId = PropertyId,
        PropertyNavigation = new Property { Id = PropertyId, OwnerId = OwnerId, Title = "Test Property" }
    };

    private static Complaint MakeComplaint(int status) => new()
    {
        Id = Guid.NewGuid(),
        LeaseId = LeaseId,
        TenantId = TenantId,
        OwnerId = OwnerId,
        PropertyId = PropertyId,
        CreatedBy = TenantId,
        StatusId = status,
        ComplaintTypeId = 1,
        PriorityId = 2,
        Subject = "Test subject",
        Description = "Test description",
        ComplaintType = new ComplaintType { Id = 1, Name = "Maintenance" },
        Priority = new ComplaintPriority { Id = 2, Name = "Medium" },
        Status = new ComplaintStatus { Id = status, Name = status.ToString() },
        Property = new Property { Id = PropertyId, Title = "Test Property", OwnerId = OwnerId },
        Tenant = new User { Id = TenantId, FirstName = "John", LastName = "Doe" },
        Comments = new List<ComplaintComment>()
    };

    private static CreateComplaintDto ValidCreateDto() => new()
    {
        LeaseId = LeaseId,
        CategoryId = 1,
        PriorityId = 2,
        Subject = "Leaking tap",
        Description = "The tap has been leaking for two days."
    };

    private static User MakeTenantUser(Guid id) => new()
    {
        Id = id,
        FirstName = "John",
        LastName = "Doe",
        UserRoles = new List<UserRole>
        {
            new() { RoleId = Role.Tenant, DeletedAt = null }
        }
    };

    private static User MakeOwnerUser(Guid id) => new()
    {
        Id = id,
        FirstName = "Jane",
        LastName = "Smith",
        UserRoles = new List<UserRole>
        {
            new() { RoleId = Role.Owner, DeletedAt = null }
        }
    };
}
