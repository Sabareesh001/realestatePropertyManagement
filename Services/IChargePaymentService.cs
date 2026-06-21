using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for Charge and Payment business operations on leases.
/// </summary>
public interface IChargePaymentService
{
    /// <summary>
    /// Applies a new charge to a lease (called by the owner).
    /// </summary>
    /// <param name="ownerId">The unique identifier of the property owner.</param>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="dto">The charge creation details.</param>
    /// <returns>A response DTO representing the created charge.</returns>
    Task<ChargeResponseDto> ApplyChargeAsync(Guid ownerId, Guid leaseId, CreateChargeDto dto);

    /// <summary>
    /// Retrieves all charges for a specific lease with role-based authorization.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="userId">The unique identifier of the requesting user.</param>
    /// <param name="roles">The roles of the requesting user.</param>
    /// <returns>A collection of charge response DTOs.</returns>
    Task<IEnumerable<ChargeResponseDto>> GetChargesByLeaseIdAsync(Guid leaseId, Guid userId, IEnumerable<string> roles);

    /// <summary>
    /// Retrieves a specific charge by its identifier with role-based authorization.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease the charge belongs to.</param>
    /// <param name="chargeId">The unique identifier of the charge.</param>
    /// <param name="userId">The unique identifier of the requesting user.</param>
    /// <param name="roles">The roles of the requesting user.</param>
    /// <returns>A charge response DTO.</returns>
    Task<ChargeResponseDto> GetChargeByIdAsync(Guid leaseId, Guid chargeId, Guid userId, IEnumerable<string> roles);

    /// <summary>
    /// Records a payment against one or more charges on a lease (called by the tenant).
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant making the payment.</param>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="dto">The payment recording details.</param>
    /// <returns>A response DTO representing the recorded payment.</returns>
    Task<PaymentResponseDto> RecordPaymentAsync(Guid tenantId, Guid leaseId, RecordPaymentDto dto);

    /// <summary>
    /// Retrieves all payments for a specific lease with role-based authorization.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="userId">The unique identifier of the requesting user.</param>
    /// <param name="roles">The roles of the requesting user.</param>
    /// <returns>A collection of payment response DTOs.</returns>
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsByLeaseIdAsync(Guid leaseId, Guid userId, IEnumerable<string> roles);
}
