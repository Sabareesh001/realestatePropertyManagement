using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for platform-wide finance queries used by the admin dashboard.
/// </summary>
public class AdminFinanceService : IAdminFinanceService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminFinanceService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    public AdminFinanceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AdminPaymentDto>> GetAllPaymentsAsync(DateTime? from, DateTime? to)
    {
        var payments = await _unitOfWork.Payments.GetAllForAdminAsync(from, to);
        return payments.Select(MapToAdminPaymentDto).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<AdminChargeDto>> GetAllChargesAsync(DateTime? from, DateTime? to)
    {
        var charges = await _unitOfWork.Charges.GetAllForAdminAsync(from, to);
        return charges.Select(MapToAdminChargeDto).ToList();
    }

    /// <inheritdoc />
    public async Task<AdminFinanceSummaryDto> GetSummaryAsync(DateTime? from, DateTime? to)
    {
        var payments = (await _unitOfWork.Payments.GetAllForAdminAsync(from, to)).ToList();

        return new AdminFinanceSummaryDto
        {
            CompanyRevenue = payments
                .Where(p => p.StatusId == PaymentStatus.Completed)
                .Sum(p => p.PlatformFeeAmount ?? 0),
            GrossVolume = payments
                .Where(p => p.StatusId == PaymentStatus.Completed)
                .Sum(p => p.Amount ?? 0),
            PendingAmount = payments
                .Where(p => p.StatusId == PaymentStatus.Pending)
                .Sum(p => p.Amount ?? 0),
            PaymentCount = payments.Count,
            CompletedCount = payments.Count(p => p.StatusId == PaymentStatus.Completed),
            FailedCount = payments.Count(p => p.StatusId == PaymentStatus.Failed),
            RefundedCount = payments.Count(p => p.StatusId == PaymentStatus.Refunded)
        };
    }

    /// <summary>
    /// Maps a Payment entity to an <see cref="AdminPaymentDto"/>, resolving lease/property/owner/tenant context.
    /// </summary>
    /// <param name="payment">The payment entity with charge, lease, property and user navigations loaded.</param>
    /// <returns>An admin payment DTO.</returns>
    private static AdminPaymentDto MapToAdminPaymentDto(Payment payment)
    {
        var lease = payment.ChargePayments
            .Select(cp => cp.Charge)
            .Where(c => c != null)
            .SelectMany(c => c.Leases)
            .FirstOrDefault();

        var property = lease?.PropertyNavigation;
        var owner = property?.Owner;
        var tenant = lease?.Tenant;

        return new AdminPaymentDto
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
            CreatedAt = payment.CreatedAt,
            PlatformFee = payment.PlatformFeeAmount,
            LeaseId = lease?.Id ?? Guid.Empty,
            PropertyId = property?.Id,
            OwnerId = property?.OwnerId,
            OwnerName = FullName(owner),
            TenantId = lease?.TenantId,
            TenantEmail = tenant?.Email
        };
    }

    /// <summary>
    /// Maps a Charge entity to an <see cref="AdminChargeDto"/>, resolving lease/property/owner/tenant context.
    /// </summary>
    /// <param name="charge">The charge entity with payment, lease, property and user navigations loaded.</param>
    /// <returns>An admin charge DTO.</returns>
    private static AdminChargeDto MapToAdminChargeDto(Charge charge)
    {
        var amountPaid = charge.ChargePayments
            .Where(cp => cp.Payment != null && cp.Payment.StatusId == PaymentStatus.Completed)
            .Sum(cp => cp.AmountApplied ?? 0);

        var lease = charge.Leases.FirstOrDefault();
        var property = lease?.PropertyNavigation;
        var owner = property?.Owner;
        var tenant = lease?.Tenant;

        return new AdminChargeDto
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
            UpdatedAt = charge.UpdatedAt,
            LeaseId = lease?.Id ?? Guid.Empty,
            PropertyId = property?.Id,
            PropertyTitle = property?.Title,
            OwnerId = property?.OwnerId,
            OwnerName = FullName(owner),
            TenantId = lease?.TenantId,
            TenantName = FullName(tenant),
            TenantEmail = tenant?.Email
        };
    }

    /// <summary>
    /// Builds a display name from a user's first and last name, or null when the user is unavailable.
    /// </summary>
    /// <param name="user">The user entity, if any.</param>
    /// <returns>The trimmed full name, or null.</returns>
    private static string? FullName(User? user)
    {
        if (user == null)
        {
            return null;
        }

        return $"{user.FirstName} {user.LastName}".Trim();
    }
}
