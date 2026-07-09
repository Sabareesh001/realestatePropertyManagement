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
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserVerificationController"/> class.
    /// </summary>
    /// <param name="userVerificationService">The user verification service.</param>
    /// <param name="env">The web host environment for resolving the web root path.</param>
    public UserVerificationController(IUserVerificationService userVerificationService, IWebHostEnvironment env)
    {
        _userVerificationService = userVerificationService;
        _env = env;
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
    /// Retrieves a page of pending user verification requests (Admin only).
    /// </summary>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A paged result of pending user verification requests.</returns>
    /// <response code="200">Pending verifications retrieved successfully.</response>
    /// <response code="403">If user is not an administrator.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<ActionResult<PagedResultDto<UserVerificationResponseDto>>> GetPendingVerifications([FromQuery] PaginationParams pagination)
    {
        var verifications = await _userVerificationService.GetPendingVerificationsAsync(pagination);
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

    /// <summary>
    /// Uploads a verification document PDF and returns a permanent URL.
    /// </summary>
    /// <param name="file">The PDF file to upload (max 10 MB).</param>
    /// <returns>An object containing the URL of the stored document.</returns>
    /// <response code="200">File uploaded successfully; returns <c>{ "url": "..." }</c>.</response>
    /// <response code="400">If no file is provided, the file is not a PDF, or the file exceeds 10 MB.</response>
    /// <response code="401">If the user is not authenticated.</response>
    [Authorize]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
    [HttpPost("upload-document")]
    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "No file was provided." });

        if (file.ContentType != "application/pdf")
            return BadRequest(new { detail = "Only PDF files are accepted." });

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest(new { detail = "File size must not exceed 10 MB." });

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsDir = Path.Combine(webRoot, "uploads", "verificationdocs");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}.pdf";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/verificationdocs/{fileName}";
        return Ok(new { url });
    }

}
