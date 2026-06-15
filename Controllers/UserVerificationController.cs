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
/// API controller for managing user verification requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserVerificationController : BaseApiController
{
    private readonly IUserVerificationService _userVerificationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserVerificationController"/> class.
    /// </summary>
    /// <param name="userVerificationService">The user verification service.</param>
    public UserVerificationController(IUserVerificationService userVerificationService)
    {
        _userVerificationService = userVerificationService;
    }

    /// <summary>
    /// Submits a user verification request with required documents.
    /// </summary>
    /// <param name="dto">The submission data containing the list of documents.</param>
    /// <returns>The created user verification request details.</returns>
    /// <response code="201">Request successfully submitted.</response>
    /// <response code="400">If request is invalid or user already has a pending or verified status.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize]
    [HttpPost("submit")]
    public async Task<ActionResult<UserVerificationResponseDto>> Submit([FromBody] SubmitVerificationDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _userVerificationService.SubmitForVerificationAsync(userId, dto);
        return CreatedAtAction(nameof(GetStatus), result);
    }

    /// <summary>
    /// Retrieves the current user's verification status.
    /// </summary>
    /// <returns>The verification status name (e.g. "Unverified", "Pending", "Verified", "Rejected").</returns>
    /// <response code="200">Status retrieved successfully.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize]
    [HttpGet("status")]
    public async Task<ActionResult<object>> GetStatus()
    {
        var userId = GetCurrentUserId();
        var status = await _userVerificationService.GetVerificationStatusAsync(userId);
        return Ok(new { status });
    }

    /// <summary>
    /// Retrieves all pending user verification requests (Admin only).
    /// </summary>
    /// <returns>A collection of pending user verification requests.</returns>
    /// <response code="200">Pending verifications retrieved successfully.</response>
    /// <response code="403">If user is not an administrator.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<UserVerificationResponseDto>>> GetPendingVerifications()
    {
        var verifications = await _userVerificationService.GetPendingVerificationsAsync();
        return Ok(verifications);
    }

    /// <summary>
    /// Approves a pending user verification request (Admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the verification request.</param>
    /// <param name="dto">The approval request details containing optional remarks.</param>
    /// <returns>The updated verification request details.</returns>
    /// <response code="200">Verification approved successfully.</response>
    /// <response code="400">If the request is not pending or invalid.</response>
    /// <response code="403">If user is not an administrator.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/verify")]
    public async Task<ActionResult<UserVerificationResponseDto>> Verify(Guid id, [FromBody] VerifyRequestDto dto)
    {
        var adminId = GetCurrentUserId();
        var result = await _userVerificationService.ApproveVerificationAsync(adminId, id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Rejects a pending user verification request (Admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the verification request.</param>
    /// <param name="dto">The rejection request details containing remarks/reasons.</param>
    /// <returns>The updated verification request details.</returns>
    /// <response code="200">Verification rejected successfully.</response>
    /// <response code="400">If the request is not pending or invalid.</response>
    /// <response code="403">If user is not an administrator.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<UserVerificationResponseDto>> Reject(Guid id, [FromBody] VerifyRequestDto dto)
    {
        var adminId = GetCurrentUserId();
        var result = await _userVerificationService.RejectVerificationAsync(adminId, id, dto);
        return Ok(result);
    }

}
