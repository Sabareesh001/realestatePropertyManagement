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
/// API controller for managing lease proposals.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeaseProposalController : BaseApiController
{
    private readonly ILeaseProposalService _leaseProposalService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseProposalController"/> class.
    /// </summary>
    /// <param name="leaseProposalService">The lease proposal service.</param>
    public LeaseProposalController(ILeaseProposalService leaseProposalService)
    {
        _leaseProposalService = leaseProposalService ?? throw new ArgumentNullException(nameof(leaseProposalService));
    }

    /// <summary>
    /// Submits a new lease proposal to rent a property. Requires the user to be verified.
    /// </summary>
    /// <param name="dto">The lease proposal details.</param>
    /// <returns>The created lease proposal details.</returns>
    /// <response code="201">Lease proposal successfully submitted.</response>
    /// <response code="400">If user is not verified or proposal is invalid.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<LeaseProposalResponseDto>> CreateLeaseProposal([FromBody] CreateLeaseProposalDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _leaseProposalService.CreateLeaseProposalAsync(userId, dto);
        return CreatedAtAction(null, result);
    }

    /// <summary>
    /// Submits a draft lease proposal. Requires the user to be verified.
    /// </summary>
    /// <param name="id">The unique identifier of the lease proposal.</param>
    /// <returns>The submitted lease proposal details.</returns>
    /// <response code="200">Lease proposal successfully submitted.</response>
    /// <response code="400">If user is not verified or proposal is not in Draft status.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not authorized to submit this proposal.</response>
    /// <response code="404">If lease proposal is not found.</response>
    [Authorize]
    [HttpPost("{id}/submit")]
    public async Task<ActionResult<LeaseProposalResponseDto>> SubmitLeaseProposal(Guid id)
    {
        var userId = GetCurrentUserId();
        var result = await _leaseProposalService.SubmitLeaseProposalAsync(userId, id);
        return Ok(result);
    }

    /// <summary>
    /// Accepts a submitted lease proposal (called by the owner of the property).
    /// </summary>
    /// <param name="id">The unique identifier of the lease proposal.</param>
    /// <returns>The accepted lease proposal details.</returns>
    /// <response code="200">Lease proposal successfully accepted.</response>
    /// <response code="400">If proposal status is not Submitted.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not the owner of the property.</response>
    /// <response code="404">If the lease proposal or property is not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpPut("{id:guid}/accept")]
    public async Task<ActionResult<LeaseProposalResponseDto>> AcceptProposal(Guid id)
    {
        var ownerId = GetCurrentUserId();
        var result = await _leaseProposalService.ReviewProposalAsync(ownerId, id, accept: true);
        return Ok(result);
    }

    /// <summary>
    /// Rejects a submitted lease proposal (called by the owner of the property).
    /// </summary>
    /// <param name="id">The unique identifier of the lease proposal.</param>
    /// <returns>The rejected lease proposal details.</returns>
    /// <response code="200">Lease proposal successfully rejected.</response>
    /// <response code="400">If proposal status is not Submitted.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not the owner of the property.</response>
    /// <response code="404">If the lease proposal or property is not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpPut("{id:guid}/reject")]
    public async Task<ActionResult<LeaseProposalResponseDto>> RejectProposal(Guid id)
    {
        var ownerId = GetCurrentUserId();
        var result = await _leaseProposalService.ReviewProposalAsync(ownerId, id, accept: false);
        return Ok(result);
    }

    /// <summary>
    /// Cancels/withdraws a draft or submitted lease proposal (called by the tenant).
    /// </summary>
    /// <param name="id">The unique identifier of the lease proposal.</param>
    /// <returns>The cancelled lease proposal details.</returns>
    /// <response code="200">Lease proposal successfully cancelled.</response>
    /// <response code="400">If proposal is not in Draft or Submitted status.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not the tenant associated with the proposal.</response>
    /// <response code="404">If the lease proposal is not found.</response>
    [Authorize(Roles = "Tenant")]
    [HttpPut("{id:guid}/cancel")]
    public async Task<ActionResult<LeaseProposalResponseDto>> CancelProposal(Guid id)
    {
        var tenantId = GetCurrentUserId();
        var result = await _leaseProposalService.CancelProposalAsync(tenantId, id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all lease proposals submitted by the currently logged-in tenant.
    /// </summary>
    /// <returns>A list of submitted lease proposals.</returns>
    /// <response code="200">Lease proposals retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize(Roles = "Tenant")]
    [HttpGet("my-requests")]
    public async Task<ActionResult<IEnumerable<LeaseProposalResponseDto>>> GetMyRequests()
    {
        var userId = GetCurrentUserId();
        var result = await _leaseProposalService.GetMyRequestsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all incoming lease proposals for properties owned by the currently logged-in owner.
    /// </summary>
    /// <returns>A list of received lease proposals with tenant details.</returns>
    /// <response code="200">Lease proposals retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    /// <response code="403">If user is not in the Owner role.</response>
    [Authorize(Roles = "Owner")]
    [HttpGet("received-requests")]
    public async Task<ActionResult<IEnumerable<LeaseProposalResponseDto>>> GetReceivedRequests()
    {
        var userId = GetCurrentUserId();
        var result = await _leaseProposalService.GetReceivedRequestsAsync(userId);
        return Ok(result);
    }
}
