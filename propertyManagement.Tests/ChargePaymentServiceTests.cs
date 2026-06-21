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
/// Unit tests for the <see cref="ChargePaymentService"/> class.
/// </summary>
[TestFixture]
public class ChargePaymentServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILeaseRepository> _mockLeaseRepository;
    private Mock<IChargeRepository> _mockChargeRepository;
    private Mock<IPaymentRepository> _mockPaymentRepository;
    private ChargePaymentService _service;

    /// <summary>
    /// Sets up mocks and initializes the service before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLeaseRepository = new Mock<ILeaseRepository>();
        _mockChargeRepository = new Mock<IChargeRepository>();
        _mockPaymentRepository = new Mock<IPaymentRepository>();

        _mockUnitOfWork.Setup(u => u.Leases).Returns(_mockLeaseRepository.Object);
        _mockUnitOfWork.Setup(u => u.Charges).Returns(_mockChargeRepository.Object);
        _mockUnitOfWork.Setup(u => u.Payments).Returns(_mockPaymentRepository.Object);

        // Transaction defaults
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        _service = new ChargePaymentService(_mockUnitOfWork.Object);
    }

    #region ApplyChargeAsync Tests

    /// <summary>
    /// Verifies that applying a charge succeeded when valid inputs are provided.
    /// </summary>
    [Test]
    public async Task ApplyChargeAsync_ValidInputs_Succeeds()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Active,
            PropertyNavigation = new Property { OwnerId = ownerId }
        };

        var dto = new CreateChargeDto
        {
            ChargeTypeId = ChargeType.MonthlyRent,
            Amount = 1500.00m,
            Description = "June Rent",
            DueDate = DateTime.Today.AddDays(5)
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.CreateAsync(It.IsAny<Charge>())).ReturnsAsync((Charge c) => c);

        var result = await _service.ApplyChargeAsync(ownerId, leaseId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Amount, Is.EqualTo(dto.Amount));
        Assert.That(result.StatusId, Is.EqualTo(ChargeStatus.Pending));
        
        _mockChargeRepository.Verify(r => r.CreateAsync(It.Is<Charge>(c => 
            c.Amount == dto.Amount &&
            c.StatusId == ChargeStatus.Pending &&
            c.Leases.Contains(lease))), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that applying a charge throws KeyNotFoundException if lease does not exist.
    /// </summary>
    [Test]
    public void ApplyChargeAsync_LeaseNotFound_ThrowsKeyNotFoundException()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var dto = new CreateChargeDto { ChargeTypeId = 1, Amount = 100, DueDate = DateTime.Today };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync((Lease?)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _service.ApplyChargeAsync(ownerId, leaseId, dto));
        Assert.That(ex.Message, Is.EqualTo("Lease not found."));
    }

    /// <summary>
    /// Verifies that applying a charge throws UnauthorizedAccessException if requesting user is not the owner.
    /// </summary>
    [Test]
    public void ApplyChargeAsync_NotOwner_ThrowsUnauthorizedAccessException()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Active,
            PropertyNavigation = new Property { OwnerId = Guid.NewGuid() } // different owner
        };
        var dto = new CreateChargeDto { ChargeTypeId = 1, Amount = 100, DueDate = DateTime.Today };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _service.ApplyChargeAsync(ownerId, leaseId, dto));
        Assert.That(ex.Message, Is.EqualTo("You are not the owner of the property associated with this lease."));
    }

    /// <summary>
    /// Verifies that applying a charge throws InvalidOperationException if the lease is not active.
    /// </summary>
    [Test]
    public void ApplyChargeAsync_LeaseNotActive_ThrowsInvalidOperationException()
    {
        var ownerId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            StatusId = LeaseStatus.Draft, // not active
            PropertyNavigation = new Property { OwnerId = ownerId }
        };
        var dto = new CreateChargeDto { ChargeTypeId = 1, Amount = 100, DueDate = DateTime.Today };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.ApplyChargeAsync(ownerId, leaseId, dto));
        Assert.That(ex.Message, Is.EqualTo("Charges can only be applied to active leases."));
    }

    #endregion

    #region RecordPaymentAsync Tests

    /// <summary>
    /// Verifies that recording a valid payment updates the charge status to Paid and returns the response.
    /// </summary>
    [Test]
    public async Task RecordPaymentAsync_FullPayment_Succeeds()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        
        var lease = new Lease
        {
            Id = leaseId,
            TenantId = tenantId,
            StatusId = LeaseStatus.Active
        };

        var charge = new Charge
        {
            Id = chargeId,
            Amount = 1000.00m,
            StatusId = ChargeStatus.Pending
        };
        charge.Leases.Add(lease);

        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN111",
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = chargeId, Amount = 1000.00m }
            }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync(charge);
        _mockPaymentRepository.Setup(r => r.CreateAsync(It.IsAny<Payment>())).ReturnsAsync((Payment p) => p);

        var result = await _service.RecordPaymentAsync(tenantId, leaseId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Amount, Is.EqualTo(1000.00m));
        Assert.That(charge.StatusId, Is.EqualTo(ChargeStatus.Paid));

        _mockPaymentRepository.Verify(r => r.CreateAsync(It.Is<Payment>(p => p.Amount == 1000.00m)), Times.Once);
        _mockChargeRepository.Verify(r => r.UpdateAsync(charge), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that recording a payment with amount less than balance updates charge status to PartiallyPaid.
    /// </summary>
    [Test]
    public async Task RecordPaymentAsync_PartialPayment_Succeeds()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        
        var lease = new Lease
        {
            Id = leaseId,
            TenantId = tenantId,
            StatusId = LeaseStatus.Active
        };

        var charge = new Charge
        {
            Id = chargeId,
            Amount = 1000.00m,
            StatusId = ChargeStatus.Pending
        };
        charge.Leases.Add(lease);

        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN111",
            CurrencyId = 1,
            ChargeAllocations = new List<ChargeAllocationDto>
            {
                new() { ChargeId = chargeId, Amount = 400.00m }
            }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync(charge);

        var result = await _service.RecordPaymentAsync(tenantId, leaseId, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(charge.StatusId, Is.EqualTo(ChargeStatus.PartiallyPaid));
        _mockChargeRepository.Verify(r => r.UpdateAsync(charge), Times.Once);
    }

    /// <summary>
    /// Verifies that recording a payment for a lease that does not exist throws KeyNotFoundException.
    /// </summary>
    [Test]
    public void RecordPaymentAsync_LeaseNotFound_ThrowsKeyNotFoundException()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var dto = new RecordPaymentDto();

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync((Lease?)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _service.RecordPaymentAsync(tenantId, leaseId, dto));
        Assert.That(ex.Message, Is.EqualTo("Lease not found."));
    }

    /// <summary>
    /// Verifies that recording a payment by a user who is not the tenant throws UnauthorizedAccessException.
    /// </summary>
    [Test]
    public void RecordPaymentAsync_NotTenant_ThrowsUnauthorizedAccessException()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            TenantId = Guid.NewGuid(), // different tenant
            StatusId = LeaseStatus.Active
        };
        var dto = new RecordPaymentDto();

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _service.RecordPaymentAsync(tenantId, leaseId, dto));
        Assert.That(ex.Message, Is.EqualTo("You are not the tenant associated with this lease."));
    }

    /// <summary>
    /// Verifies that recording a payment for a lease that is not active throws InvalidOperationException.
    /// </summary>
    [Test]
    public void RecordPaymentAsync_LeaseNotActive_ThrowsInvalidOperationException()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            TenantId = tenantId,
            StatusId = LeaseStatus.Submitted // not active
        };
        var dto = new RecordPaymentDto();

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.RecordPaymentAsync(tenantId, leaseId, dto));
        Assert.That(ex.Message, Is.EqualTo("Payments can only be recorded against active leases."));
    }

    /// <summary>
    /// Verifies that recording a payment for a charge that does not exist throws KeyNotFoundException and rolls back transaction.
    /// </summary>
    [Test]
    public void RecordPaymentAsync_ChargeNotFound_ThrowsKeyNotFoundExceptionAndRollsBack()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        
        var lease = new Lease { Id = leaseId, TenantId = tenantId, StatusId = LeaseStatus.Active };
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN1",
            ChargeAllocations = new List<ChargeAllocationDto> { new() { ChargeId = chargeId, Amount = 100 } }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync((Charge?)null);

        var ex = Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _service.RecordPaymentAsync(tenantId, leaseId, dto));
        
        Assert.That(ex.Message, Is.EqualTo($"Charge with ID '{chargeId}' not found."));
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that recording a payment for a charge that does not belong to the lease throws InvalidOperationException and rolls back transaction.
    /// </summary>
    [Test]
    public void RecordPaymentAsync_ChargeDoesNotBelongToLease_ThrowsInvalidOperationExceptionAndRollsBack()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        
        var lease = new Lease { Id = leaseId, TenantId = tenantId, StatusId = LeaseStatus.Active };
        var charge = new Charge { Id = chargeId, Amount = 100 }; // Leases list does not contain leaseId
        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN1",
            ChargeAllocations = new List<ChargeAllocationDto> { new() { ChargeId = chargeId, Amount = 100 } }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync(charge);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.RecordPaymentAsync(tenantId, leaseId, dto));
        
        Assert.That(ex.Message, Is.EqualTo($"Charge '{chargeId}' does not belong to the specified lease."));
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that recording a payment for a charge that is already Paid or Cancelled throws InvalidOperationException.
    /// </summary>
    /// <param name="statusId">The invalid charge status ID under test.</param>
    [TestCase(ChargeStatus.Paid)]
    [TestCase(ChargeStatus.Cancelled)]
    public void RecordPaymentAsync_ChargeAlreadyPaidOrCancelled_ThrowsInvalidOperationExceptionAndRollsBack(int statusId)
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        
        var lease = new Lease { Id = leaseId, TenantId = tenantId, StatusId = LeaseStatus.Active };
        var charge = new Charge { Id = chargeId, Amount = 100, StatusId = statusId };
        charge.Leases.Add(lease);

        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN1",
            ChargeAllocations = new List<ChargeAllocationDto> { new() { ChargeId = chargeId, Amount = 100 } }
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync(charge);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.RecordPaymentAsync(tenantId, leaseId, dto));
        
        var expectedMsg = $"Charge '{chargeId}' is already {(statusId == ChargeStatus.Paid ? "paid" : "cancelled")}.";
        Assert.That(ex.Message, Is.EqualTo(expectedMsg));
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    /// <summary>
    /// Verifies that recording a payment that exceeds the balance due throws InvalidOperationException.
    /// </summary>
    [Test]
    public void RecordPaymentAsync_PaymentExceedsBalance_ThrowsInvalidOperationExceptionAndRollsBack()
    {
        var tenantId = Guid.NewGuid();
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        
        var lease = new Lease { Id = leaseId, TenantId = tenantId, StatusId = LeaseStatus.Active };
        var charge = new Charge { Id = chargeId, Amount = 100, StatusId = ChargeStatus.Pending };
        charge.Leases.Add(lease);

        var dto = new RecordPaymentDto
        {
            PaymentMethodId = 1,
            TransactionRef = "TXN1",
            ChargeAllocations = new List<ChargeAllocationDto> { new() { ChargeId = chargeId, Amount = 150 } } // exceeds 100
        };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync(charge);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.RecordPaymentAsync(tenantId, leaseId, dto));
        
        Assert.That(ex.Message, Is.EqualTo($"Payment amount 150 exceeds the balance due 100 for charge '{chargeId}'."));
        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    #endregion

    #region GetChargesByLeaseIdAsync Tests

    /// <summary>
    /// Verifies that an Admin can retrieve charges for any lease.
    /// </summary>
    [Test]
    public async Task GetChargesByLeaseIdAsync_AdminRole_Succeeds()
    {
        var leaseId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId };
        var charges = new List<Charge> { new() { Id = Guid.NewGuid(), Amount = 100 } };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByLeaseIdAsync(leaseId)).ReturnsAsync(charges);

        var result = await _service.GetChargesByLeaseIdAsync(leaseId, Guid.NewGuid(), new[] { "Admin" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    /// <summary>
    /// Verifies that an Owner can retrieve charges for their own property lease.
    /// </summary>
    [Test]
    public async Task GetChargesByLeaseIdAsync_OwnerRole_Succeeds()
    {
        var leaseId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var lease = new Lease
        {
            Id = leaseId,
            PropertyNavigation = new Property { OwnerId = ownerId }
        };
        var charges = new List<Charge> { new() { Id = Guid.NewGuid(), Amount = 200 } };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByLeaseIdAsync(leaseId)).ReturnsAsync(charges);

        var result = await _service.GetChargesByLeaseIdAsync(leaseId, ownerId, new[] { "Owner" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    /// <summary>
    /// Verifies that a Tenant can retrieve charges for their own lease.
    /// </summary>
    [Test]
    public async Task GetChargesByLeaseIdAsync_TenantRole_Succeeds()
    {
        var leaseId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId, TenantId = tenantId };
        var charges = new List<Charge> { new() { Id = Guid.NewGuid(), Amount = 300 } };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByLeaseIdAsync(leaseId)).ReturnsAsync(charges);

        var result = await _service.GetChargesByLeaseIdAsync(leaseId, tenantId, new[] { "Tenant" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    /// <summary>
    /// Verifies that an unauthorized user cannot retrieve charges.
    /// </summary>
    [Test]
    public void GetChargesByLeaseIdAsync_UnauthorizedUser_ThrowsUnauthorizedAccessException()
    {
        var leaseId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId, TenantId = Guid.NewGuid() };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);

        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _service.GetChargesByLeaseIdAsync(leaseId, Guid.NewGuid(), new[] { "Tenant" }));
        
        Assert.That(ex.Message, Is.EqualTo("You are not authorized to access charges/payments for this lease."));
    }

    #endregion

    #region GetChargeByIdAsync Tests

    /// <summary>
    /// Verifies that a specific charge can be retrieved when user has access and charge belongs to lease.
    /// </summary>
    [Test]
    public async Task GetChargeByIdAsync_ValidInputs_Succeeds()
    {
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId, TenantId = Guid.NewGuid() };
        var charge = new Charge { Id = chargeId, Amount = 150 };
        charge.Leases.Add(lease);

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync(charge);

        var result = await _service.GetChargeByIdAsync(leaseId, chargeId, lease.TenantId.Value, new[] { "Tenant" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(chargeId));
    }

    /// <summary>
    /// Verifies that retrieving a charge throws InvalidOperationException if the charge does not belong to the specified lease.
    /// </summary>
    [Test]
    public void GetChargeByIdAsync_ChargeDoesNotBelongToLease_ThrowsInvalidOperationException()
    {
        var leaseId = Guid.NewGuid();
        var chargeId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId, TenantId = Guid.NewGuid() };
        var charge = new Charge { Id = chargeId, Amount = 150 }; // Leases list empty

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockChargeRepository.Setup(r => r.GetByIdWithPaymentsAsync(chargeId)).ReturnsAsync(charge);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _service.GetChargeByIdAsync(leaseId, chargeId, lease.TenantId.Value, new[] { "Tenant" }));
        
        Assert.That(ex.Message, Is.EqualTo("Charge does not belong to the specified lease."));
    }

    #endregion

    #region GetPaymentsByLeaseIdAsync Tests

    /// <summary>
    /// Verifies that retrieving payments returns correct result when authorized.
    /// </summary>
    [Test]
    public async Task GetPaymentsByLeaseIdAsync_AuthorizedUser_Succeeds()
    {
        var leaseId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var lease = new Lease { Id = leaseId, TenantId = tenantId };
        var payments = new List<Payment> { new() { Id = Guid.NewGuid(), Amount = 500 } };

        _mockLeaseRepository.Setup(r => r.GetByIdAsync(leaseId)).ReturnsAsync(lease);
        _mockPaymentRepository.Setup(r => r.GetByLeaseIdAsync(leaseId)).ReturnsAsync(payments);

        var result = await _service.GetPaymentsByLeaseIdAsync(leaseId, tenantId, new[] { "Tenant" });

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    #endregion
}
