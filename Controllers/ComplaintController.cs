using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller for managing tenant complaints against properties/leases.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ComplaintController : BaseApiController
{
    private readonly IComplaintService _complaintService;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Initializes a new instance of <see cref="ComplaintController"/>.
    /// </summary>
    /// <param name="complaintService">The complaint service.</param>
    /// <param name="env">The web host environment (for file uploads).</param>
    public ComplaintController(IComplaintService complaintService, IWebHostEnvironment env)
    {
        _complaintService = complaintService ?? throw new ArgumentNullException(nameof(complaintService));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    /// <summary>
    /// Creates a new complaint. Caller must be the tenant of an active lease.
    /// </summary>
    /// <param name="dto">The complaint creation payload.</param>
    /// <returns>The created complaint.</returns>
    /// <response code="201">Complaint created successfully.</response>
    /// <response code="400">Validation failed or lease is not active.</response>
    /// <response code="403">Caller is not the tenant of the lease.</response>
    [Authorize(Roles = "Tenant")]
    [HttpPost]
    public async Task<ActionResult<ComplaintResponseDto>> CreateComplaint([FromBody] CreateComplaintDto dto)
    {
        var tenantId = GetCurrentUserId();
        var result = await _complaintService.CreateComplaintAsync(tenantId, dto);
        return CreatedAtAction(nameof(GetComplaintById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves all complaints created by the authenticated user.
    /// </summary>
    /// <returns>A list of complaints (empty comment lists).</returns>
    /// <response code="200">Complaints retrieved successfully.</response>
    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ComplaintResponseDto>>> GetMyComplaints()
    {
        var userId = GetCurrentUserId();
        var result = await _complaintService.GetMyComplaintsAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all complaints on properties owned by the authenticated owner.
    /// </summary>
    /// <returns>A list of complaints (empty comment lists).</returns>
    /// <response code="200">Complaints retrieved successfully.</response>
    [Authorize(Roles = "Owner")]
    [HttpGet("received")]
    public async Task<ActionResult<IEnumerable<ComplaintResponseDto>>> GetReceivedComplaints()
    {
        var ownerId = GetCurrentUserId();
        var result = await _complaintService.GetReceivedComplaintsAsync(ownerId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all complaints in the system. Admin only.
    /// </summary>
    /// <returns>A list of all complaints (empty comment lists).</returns>
    /// <response code="200">All complaints retrieved successfully.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComplaintResponseDto>>> GetAllComplaints()
    {
        var result = await _complaintService.GetAllComplaintsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a single complaint with full comment thread. Accessible to tenant, owner, or admin.
    /// </summary>
    /// <param name="id">The complaint identifier.</param>
    /// <returns>The complaint with full comments.</returns>
    /// <response code="200">Complaint retrieved successfully.</response>
    /// <response code="403">Caller is not a participant of this complaint.</response>
    /// <response code="404">Complaint not found.</response>
    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ComplaintResponseDto>> GetComplaintById(Guid id)
    {
        var userId = GetCurrentUserId();
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var result = await _complaintService.GetComplaintByIdAsync(id, userId, roles);
        return Ok(result);
    }

    /// <summary>
    /// Updates the status of a complaint, applying the state machine rules.
    /// An optional note is appended as a comment.
    /// </summary>
    /// <param name="id">The complaint identifier.</param>
    /// <param name="dto">The status update payload.</param>
    /// <returns>The updated complaint with refreshed comments.</returns>
    /// <response code="200">Status updated successfully.</response>
    /// <response code="400">Validation failed or invalid status transition.</response>
    /// <response code="403">Caller does not have permission for this transition.</response>
    /// <response code="404">Complaint not found.</response>
    [Authorize]
    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ComplaintResponseDto>> UpdateStatus(Guid id, [FromBody] UpdateComplaintStatusDto dto)
    {
        var userId = GetCurrentUserId();
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var result = await _complaintService.UpdateStatusAsync(id, userId, roles, dto);
        return Ok(result);
    }

    /// <summary>
    /// Adds a comment to a complaint thread. Accessible to tenant, owner, or admin.
    /// Blocked when the complaint is Closed or Cancelled.
    /// </summary>
    /// <param name="id">The complaint identifier.</param>
    /// <param name="dto">The comment payload.</param>
    /// <returns>The created comment.</returns>
    /// <response code="201">Comment added successfully.</response>
    /// <response code="400">Complaint is in a terminal status.</response>
    /// <response code="403">Caller is not a participant.</response>
    /// <response code="404">Complaint not found.</response>
    [Authorize]
    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<ComplaintCommentDto>> AddComment(Guid id, [FromBody] AddCommentDto dto)
    {
        var userId = GetCurrentUserId();
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var result = await _complaintService.AddCommentAsync(id, userId, roles, dto);
        return CreatedAtAction(nameof(GetComplaintById), new { id }, result);
    }

    /// <summary>
    /// Uploads a complaint attachment (PDF or image, max 10 MB) and returns a permanent URL.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <returns>An object containing the URL of the stored file.</returns>
    /// <response code="200">File uploaded successfully; returns <c>{ "url": "..." }</c>.</response>
    /// <response code="400">No file provided, unsupported type, or file exceeds 10 MB.</response>
    [Authorize]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)]
    [HttpPost("upload-document")]
    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { detail = "No file was provided." });

        var allowedTypes = new[]
        {
            "application/pdf",
            "image/jpeg",
            "image/png",
            "image/gif",
            "image/webp"
        };

        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { detail = "Only PDF and image files (JPEG, PNG, GIF, WebP) are accepted." });

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest(new { detail = "File size must not exceed 10 MB." });

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsDir = Path.Combine(webRoot, "uploads", "complaintdocs");
        Directory.CreateDirectory(uploadsDir);

        var ext = file.ContentType == "application/pdf"
            ? ".pdf"
            : Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/complaintdocs/{fileName}";
        return Ok(new { url });
    }
}
