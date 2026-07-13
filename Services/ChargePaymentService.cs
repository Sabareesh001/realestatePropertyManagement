using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for Charge and Payment business operations on leases.
/// </summary>
public class ChargePaymentService : IChargePaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChargePaymentService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    public ChargePaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Applies a new charge to a lease (called by the owner).
    /// </summary>
    /// <param name="ownerId">The unique identifier of the property owner.</param>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="dto">The charge creation details.</param>
    /// <returns>A response DTO representing the created charge.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the lease is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the user is not the owner or the lease is not active.</exception>
    public async Task<ChargeResponseDto> ApplyChargeAsync(Guid ownerId, Guid leaseId, CreateChargeDto dto)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        if (lease.PropertyNavigation?.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("You are not the owner of the property associated with this lease.");
        }

        if (lease.StatusId != LeaseStatus.Active)
        {
            throw new InvalidOperationException("Charges can only be applied to active leases.");
        }

        var charge = new Charge
        {
            Id = Guid.NewGuid(),
            ChargeTypeId = dto.ChargeTypeId,
            Amount = dto.Amount,
            Desc = dto.Description,
            DueDate = DateTime.SpecifyKind(dto.DueDate, DateTimeKind.Unspecified),
            StatusId = ChargeStatus.Pending,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        // Link the charge to the lease via the many-to-many relationship
        charge.Leases.Add(lease);

        await _unitOfWork.Charges.CreateAsync(charge);
        await _unitOfWork.SaveChangesAsync();

        var createdCharge = await _unitOfWork.Charges.GetByIdAsync(charge.Id);
        return MapToChargeResponseDto(createdCharge ?? charge);
    }

    /// <summary>
    /// Retrieves all charges for a specific lease with role-based authorization.
    /// </summary>
    public async Task<PagedResultDto<ChargeResponseDto>> GetChargesByLeaseIdAsync(Guid leaseId, Guid userId, IEnumerable<string> roles, PaginationParams pagination)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        AuthorizeLeaseAccess(lease, userId, roles);

        var charges = await _unitOfWork.Charges.GetByLeaseIdAsync(leaseId, pagination.PageNumber, pagination.PageSize);
        return charges.Select(MapToChargeResponseDto);
    }

    /// <summary>
    /// Retrieves a specific charge by its identifier with role-based authorization.
    /// </summary>
    public async Task<ChargeResponseDto> GetChargeByIdAsync(Guid leaseId, Guid chargeId, Guid userId, IEnumerable<string> roles)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        AuthorizeLeaseAccess(lease, userId, roles);

        var charge = await _unitOfWork.Charges.GetByIdWithPaymentsAsync(chargeId);
        if (charge == null)
        {
            throw new KeyNotFoundException("Charge not found.");
        }

        // Verify charge belongs to the specified lease
        if (!charge.Leases.Any(l => l.Id == leaseId))
        {
            throw new InvalidOperationException("Charge does not belong to the specified lease.");
        }

        return MapToChargeResponseDto(charge);
    }

    /// <summary>
    /// Records a payment against one or more charges on a lease (called by the tenant).
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant making the payment.</param>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="dto">The payment recording details.</param>
    /// <returns>A response DTO representing the recorded payment.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the lease is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public async Task<PaymentResponseDto> RecordPaymentAsync(Guid tenantId, Guid leaseId, RecordPaymentDto dto)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        if (lease.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("You are not the tenant associated with this lease.");
        }

        if (lease.StatusId != LeaseStatus.Active)
        {
            throw new InvalidOperationException("Payments can only be recorded against active leases.");
        }

        await _unitOfWork.BeginTransactionAsync();

        var totalPaymentAmount = dto.ChargeAllocations.Sum(a => a.Amount);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = totalPaymentAmount,
            TransactionRef = dto.TransactionRef,
            PaymentMethodId = dto.PaymentMethodId,
            CurrencyId = dto.CurrencyId,
            PaidBy = tenantId,
            PaidAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
            StatusId = PaymentStatus.Completed,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _unitOfWork.Payments.CreateAsync(payment);

        // Process each charge allocation
        foreach (var allocation in dto.ChargeAllocations)
        {
            var charge = await _unitOfWork.Charges.GetByIdWithPaymentsAsync(allocation.ChargeId);
            if (charge == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new KeyNotFoundException($"Charge with ID '{allocation.ChargeId}' not found.");
            }

            // Verify charge belongs to the specified lease
            if (!charge.Leases.Any(l => l.Id == leaseId))
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException($"Charge '{allocation.ChargeId}' does not belong to the specified lease.");
            }

            // Verify charge is in a payable status
            if (charge.StatusId == ChargeStatus.Paid || charge.StatusId == ChargeStatus.Cancelled)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException($"Charge '{allocation.ChargeId}' is already {(charge.StatusId == ChargeStatus.Paid ? "paid" : "cancelled")}.");
            }

            // Calculate balance due
            var amountAlreadyPaid = charge.ChargePayments
                .Where(cp => cp.Payment.StatusId == PaymentStatus.Completed)
                .Sum(cp => cp.AmountApplied ?? 0);
            var balanceDue = (charge.Amount ?? 0) - amountAlreadyPaid;

            if (allocation.Amount > balanceDue)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException($"Payment amount {allocation.Amount} exceeds the balance due {balanceDue} for charge '{allocation.ChargeId}'.");
            }

            // Create the charge-payment mapping
            var chargePayment = new ChargePayment
            {
                ChargeId = charge.Id,
                PaymentId = payment.Id,
                AmountApplied = allocation.Amount
            };
            charge.ChargePayments.Add(chargePayment);

            // Update charge status based on total amount paid
            var newTotalPaid = amountAlreadyPaid + allocation.Amount;
            if (newTotalPaid >= (charge.Amount ?? 0))
            {
                charge.StatusId = ChargeStatus.Paid;
            }
            else if (newTotalPaid > 0)
            {
                charge.StatusId = ChargeStatus.PartiallyPaid;
            }

            await _unitOfWork.Charges.UpdateAsync(charge);
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        var createdPayment = await _unitOfWork.Payments.GetByIdAsync(payment.Id);
        return MapToPaymentResponseDto(createdPayment ?? payment);
    }

    /// <summary>
    /// Retrieves all payments for a specific lease with role-based authorization.
    /// </summary>
    public async Task<PagedResultDto<PaymentResponseDto>> GetPaymentsByLeaseIdAsync(Guid leaseId, Guid userId, IEnumerable<string> roles, PaginationParams pagination)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        AuthorizeLeaseAccess(lease, userId, roles);

        var payments = await _unitOfWork.Payments.GetByLeaseIdAsync(leaseId, pagination.PageNumber, pagination.PageSize);
        return payments.Select(MapToPaymentResponseDto);
    }

    /// <summary>
    /// Validates that the requesting user has access to the lease based on their role.
    /// </summary>
    /// <param name="lease">The lease entity.</param>
    /// <param name="userId">The requesting user identifier.</param>
    /// <param name="roles">The roles of the requesting user.</param>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user has no access.</exception>
    private static void AuthorizeLeaseAccess(Lease lease, Guid userId, IEnumerable<string> roles)
    {
        var roleList = roles.ToList();

        if (roleList.Contains("Admin"))
        {
            return;
        }

        if (roleList.Contains("Owner") && lease.PropertyNavigation?.OwnerId == userId)
        {
            return;
        }

        if (roleList.Contains("Tenant") && lease.TenantId == userId)
        {
            return;
        }

        throw new UnauthorizedAccessException("You are not authorized to access charges/payments for this lease.");
    }

    /// <summary>
    /// Maps a Charge entity to a ChargeResponseDto.
    /// </summary>
    /// <param name="charge">The charge entity.</param>
    /// <returns>A charge response DTO.</returns>
    private static ChargeResponseDto MapToChargeResponseDto(Charge charge)
    {
        var amountPaid = charge.ChargePayments
            .Where(cp => cp.Payment != null && cp.Payment.StatusId == PaymentStatus.Completed)
            .Sum(cp => cp.AmountApplied ?? 0);

        return new ChargeResponseDto
        {
            Id = charge.Id,
            ChargeTypeId = charge.ChargeTypeId,
            ChargeTypeName = charge.ChargeType?.Name,
            Amount = charge.Amount,
            Description = charge.Desc,
            DueDate = charge.DueDate,
            StatusId = charge.StatusId,
            StatusName = charge.Status?.Name,
            AmountPaid = amountPaid,
            BalanceDue = (charge.Amount ?? 0) - amountPaid,
            CreatedAt = charge.CreatedAt,
            UpdatedAt = charge.UpdatedAt
        };
    }

    /// <summary>
    /// Maps a Payment entity to a PaymentResponseDto.
    /// </summary>
    /// <param name="payment">The payment entity.</param>
    /// <returns>A payment response DTO.</returns>
    private static PaymentResponseDto MapToPaymentResponseDto(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            Amount = payment.Amount,
            TransactionRef = payment.TransactionRef,
            PaymentMethodId = payment.PaymentMethodId,
            PaymentMethodName = payment.PaymentMethod?.Name,
            StatusId = payment.StatusId,
            StatusName = payment.Status?.Name,
            PaidBy = payment.PaidBy,
            PaidAt = payment.PaidAt,
            CurrencyId = payment.CurrencyId,
            ChargeAllocations = payment.ChargePayments.Select(cp => new ChargeAllocationResponseDto
            {
                ChargeId = cp.ChargeId,
                AmountApplied = cp.AmountApplied
            }).ToList(),
            CreatedAt = payment.CreatedAt
        };
    }
}
