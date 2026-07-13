using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for managing charges and payments on leases.
/// </summary>
[ApiController]
[Route("api/lease/{leaseId:guid}")]
public class ChargePaymentController : BaseApiController
{
    private readonly IChargePaymentService _chargePaymentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChargePaymentController"/> class.
    /// </summary>
    /// <param name="chargePaymentService">The charge and payment service.</param>
    public ChargePaymentController(IChargePaymentService chargePaymentService)
    {
        _chargePaymentService = chargePaymentService ?? throw new ArgumentNullException(nameof(chargePaymentService));
    }

    /// <summary>
    /// Applies a new charge to an active lease (called by the property owner).
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="dto">The charge creation payload.</param>
    /// <returns>The created charge details.</returns>
    /// <response code="201">Charge successfully applied.</response>
    /// <response code="400">If validation fails or lease is not active.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not the Owner of the property.</response>
    /// <response code="404">If the lease was not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpPost("charges")]
    public async Task<ActionResult<ChargeResponseDto>> ApplyCharge(Guid leaseId, [FromBody] CreateChargeDto dto)
    {
        var ownerId = GetCurrentUserId();
        var result = await _chargePaymentService.ApplyChargeAsync(ownerId, leaseId, dto);
        return CreatedAtAction(nameof(GetChargeById), new { leaseId, chargeId = result.Id }, result);
    }

    /// <summary>
    /// Retrieves a page of charges for a specific lease.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of charges for the lease.</returns>
    /// <response code="200">Charges retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user has no access to this lease.</response>
    /// <response code="404">If the lease was not found.</response>
    [Authorize]
    [HttpGet("charges")]
    public async Task<ActionResult<PagedResultDto<ChargeResponseDto>>> GetChargesByLeaseId(Guid leaseId, [FromQuery] PaginationParams pagination)
    {
        var userId = GetCurrentUserId();
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var result = await _chargePaymentService.GetChargesByLeaseIdAsync(leaseId, userId, roles, pagination);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific charge by its identifier.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="chargeId">The unique identifier of the charge.</param>
    /// <returns>The charge details.</returns>
    /// <response code="200">Charge retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user has no access to this lease.</response>
    /// <response code="404">If the lease or charge was not found.</response>
    [Authorize]
    [HttpGet("charges/{chargeId:guid}")]
    public async Task<ActionResult<ChargeResponseDto>> GetChargeById(Guid leaseId, Guid chargeId)
    {
        var userId = GetCurrentUserId();
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var result = await _chargePaymentService.GetChargeByIdAsync(leaseId, chargeId, userId, roles);
        return Ok(result);
    }

    /// <summary>
    /// Records a payment against one or more charges on a lease (called by the tenant).
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="dto">The payment recording payload.</param>
    /// <returns>The recorded payment details.</returns>
    /// <response code="201">Payment successfully recorded.</response>
    /// <response code="400">If validation fails or lease is not active.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not the Tenant of the lease.</response>
    /// <response code="404">If the lease or charge was not found.</response>
    [Authorize(Roles = "Tenant")]
    [HttpPost("payments")]
    public async Task<ActionResult<PaymentResponseDto>> RecordPayment(Guid leaseId, [FromBody] RecordPaymentDto dto)
    {
        var tenantId = GetCurrentUserId();
        var result = await _chargePaymentService.RecordPaymentAsync(tenantId, leaseId, dto);
        return CreatedAtAction(nameof(GetPaymentsByLeaseId), new { leaseId }, result);
    }

    /// <summary>
    /// Retrieves a page of payments for a specific lease.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of payments for the lease.</returns>
    /// <response code="200">Payments retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user has no access to this lease.</response>
    /// <response code="404">If the lease was not found.</response>
    [Authorize]
    [HttpGet("payments")]
    public async Task<ActionResult<PagedResultDto<PaymentResponseDto>>> GetPaymentsByLeaseId(Guid leaseId, [FromQuery] PaginationParams pagination)
    {
        var userId = GetCurrentUserId();
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var result = await _chargePaymentService.GetPaymentsByLeaseIdAsync(leaseId, userId, roles, pagination);
        return Ok(result);
    }
}
