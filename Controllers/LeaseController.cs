using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;
using propertyManagement.Filters;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for managing the multi-stage lease contract lifecycle.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeaseController : BaseApiController
{
    private readonly ILeaseService _leaseService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseController"/> class.
    /// </summary>
    /// <param name="leaseService">The lease service.</param>
    public LeaseController(ILeaseService leaseService)
    {
        _leaseService = leaseService ?? throw new ArgumentNullException(nameof(leaseService));
    }

    /// <summary>
    /// Creates a new lease in Draft or Submitted status (called by the owner).
    /// </summary>
    /// <param name="dto">The lease creation payload.</param>
    /// <returns>The created lease details.</returns>
    /// <response code="201">Lease successfully created.</response>
    /// <response code="400">If validation fails or user is not verified.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not in the Owner role.</response>
    [AuthorizeRoles("Owner")]
    [HttpPost]
    public async Task<ActionResult<LeaseResponseDto>> CreateLease([FromBody] CreateLeaseDto dto)
    {
        var ownerId = GetCurrentUserId();
        var result = await _leaseService.CreateLeaseAsync(ownerId, dto);
        return CreatedAtAction(nameof(GetLeaseById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Updates an existing lease draft or submitted lease (called by the owner).
    /// </summary>
    /// <param name="id">The unique identifier of the lease to update.</param>
    /// <param name="dto">The lease update payload.</param>
    /// <returns>The updated lease details.</returns>
    /// <response code="200">Lease updated successfully.</response>
    /// <response code="400">If validation fails.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not in the Owner role.</response>
    /// <response code="404">If the lease was not found.</response>
    [AuthorizeRoles("Owner")]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LeaseResponseDto>> UpdateLease(Guid id, [FromBody] UpdateLeaseDto dto)
    {
        var ownerId = GetCurrentUserId();
        var result = await _leaseService.UpdateLeaseAsync(ownerId, id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Verifies/approves or rejects a submitted lease agreement template (called by an admin).
    /// </summary>
    /// <param name="id">The unique identifier of the lease.</param>
    /// <param name="approve">True to approve the template (moves status to PendingSignature); false to reject.</param>
    /// <returns>The updated lease details.</returns>
    /// <response code="200">Lease template verified successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not in the Admin role.</response>
    /// <response code="404">If the lease was not found.</response>
    [AuthorizeRoles("Admin")]
    [HttpPut("{id:guid}/verify-template")]
    public async Task<ActionResult<LeaseResponseDto>> VerifyTemplate(Guid id, [FromQuery] bool approve = true)
    {
        var adminId = GetCurrentUserId();
        var result = await _leaseService.VerifyLeaseTemplateAsync(adminId, id, approve);
        return Ok(result);
    }

    /// <summary>
    /// Signs the lease and uploads the signed agreement document (called by the tenant).
    /// </summary>
    /// <param name="id">The unique identifier of the lease.</param>
    /// <param name="dto">The lease signature payload.</param>
    /// <returns>The signed lease details.</returns>
    /// <response code="200">Lease signed successfully.</response>
    /// <response code="400">If validation fails.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not in the Tenant role.</response>
    /// <response code="404">If the lease was not found.</response>
    [AuthorizeRoles("Tenant")]
    [HttpPut("{id:guid}/sign")]
    public async Task<ActionResult<LeaseResponseDto>> SignLease(Guid id, [FromBody] SignLeaseDto dto)
    {
        var tenantId = GetCurrentUserId();
        var result = await _leaseService.SignLeaseAsync(tenantId, id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Verifies/approves or rejects a tenant-signed lease agreement (called by an admin) to make it active.
    /// </summary>
    /// <param name="id">The unique identifier of the lease.</param>
    /// <param name="approve">True to activate the lease; false to reject.</param>
    /// <returns>The activated lease details.</returns>
    /// <response code="200">Lease signature verified successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not in the Admin role.</response>
    /// <response code="404">If the lease was not found.</response>
    [AuthorizeRoles("Admin")]
    [HttpPut("{id:guid}/verify-signed")]
    public async Task<ActionResult<LeaseResponseDto>> VerifySigned(Guid id, [FromQuery] bool approve = true)
    {
        var adminId = GetCurrentUserId();
        var result = await _leaseService.VerifySignedLeaseAsync(adminId, id, approve);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific lease details, validating role-based authorization.
    /// </summary>
    /// <param name="id">The unique identifier of the lease.</param>
    /// <returns>The lease details.</returns>
    /// <response code="200">Lease retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user has no access to this lease.</response>
    /// <response code="404">If the lease was not found.</response>
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeaseResponseDto>> GetLeaseById(Guid id)
    {
        var userId = GetCurrentUserId();
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        var result = await _leaseService.GetLeaseByIdAsync(id, userId, role);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all leases associated with the user based on their role.
    /// </summary>
    /// <returns>A list of leases.</returns>
    /// <response code="200">Leases retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize]
    [HttpGet("my-leases")]
    public async Task<ActionResult<IEnumerable<LeaseResponseDto>>> GetMyLeases()
    {
        var userId = GetCurrentUserId();
        var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        var result = await _leaseService.GetMyLeasesAsync(userId, role);
        return Ok(result);
    }
}
