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
/// API controller for managing property postings.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PropertyController : BaseApiController
{
    private readonly IPropertyService _propertyService;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyController"/> class.
    /// </summary>
    /// <param name="propertyService">The property service.</param>
    /// <param name="env">The web host environment for resolving the web root path.</param>
    public PropertyController(IPropertyService propertyService, IWebHostEnvironment env)
    {
        _propertyService = propertyService;
        _env = env;
    }

    /// <summary>
    /// Posts a new property listing. Requires the user to be verified.
    /// </summary>
    /// <param name="dto">The property creation details.</param>
    /// <returns>The created property details.</returns>
    /// <response code="201">Property successfully created.</response>
    /// <response code="400">If user is not verified or request is invalid.</response>
    /// <response code="401">If user is unauthorized.</response>
    [Authorize(Roles = "Owner")]
    [HttpPost]
    public async Task<ActionResult<PropertyResponseDto>> CreateProperty([FromBody] CreatePropertyDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _propertyService.CreatePropertyAsync(userId, dto);
        return CreatedAtAction(nameof(GetPropertyById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Retrieves a specific property listing by its unique identifier.
    /// Non-owners receive 404 for Draft, Submitted, or Rejected properties.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <returns>The property details.</returns>
    /// <response code="200">Returns the property details.</response>
    /// <response code="404">If the property is not found or is not publicly visible.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropertyResponseDto>> GetPropertyById(int id)
    {
        var requestingUserId = User.Identity?.IsAuthenticated == true ? GetCurrentUserId() : (Guid?)null;
        var result = await _propertyService.GetPropertyByIdAsync(id, requestingUserId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a page of property listings.
    /// </summary>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of property listings.</returns>
    /// <response code="200">Returns the page of properties.</response>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<PropertyResponseDto>>> GetAllProperties([FromQuery] PaginationParams pagination)
    {
        var result = await _propertyService.GetAllPropertiesAsync(pagination);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a page of property listings owned by the currently authenticated owner.
    /// </summary>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of property listings owned by the user.</returns>
    /// <response code="200">Returns the page of owner's properties.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not in the Owner role.</response>
    [Authorize(Roles = "Owner")]
    [HttpGet("my")]
    public async Task<ActionResult<PagedResultDto<PropertyResponseDto>>> GetMyProperties([FromQuery] PaginationParams pagination)
    {
        var userId = GetCurrentUserId();
        var result = await _propertyService.GetPropertiesByOwnerIdAsync(userId, pagination);
        return Ok(result);
    }


    /// <summary>
    /// Updates an existing property listing. Requires the authenticated user to be the owner.
    /// </summary>
    /// <param name="id">The unique identifier of the property to update.</param>
    /// <param name="dto">The updated property details.</param>
    /// <returns>The updated property details.</returns>
    /// <response code="200">Returns the updated property details.</response>
    /// <response code="400">If the input details are invalid.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not the owner of the property.</response>
    /// <response code="404">If the property is not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PropertyResponseDto>> UpdateProperty(int id, [FromBody] UpdatePropertyDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _propertyService.UpdatePropertyAsync(userId, id, dto);
        return Ok(result);
    }

    /// <summary>
    /// Deletes an existing property listing. Requires the authenticated user to be the owner.
    /// </summary>
    /// <param name="id">The unique identifier of the property to delete.</param>
    /// <returns>No Content response.</returns>
    /// <response code="204">If the property was deleted successfully.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not the owner of the property.</response>
    /// <response code="404">If the property is not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var userId = GetCurrentUserId();
        await _propertyService.DeletePropertyAsync(userId, id);
        return NoContent();
    }

    /// <summary>
    /// Submits a Draft (or Rejected) property for admin verification. Owner only.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <returns>The updated property details.</returns>
    /// <response code="200">Property successfully submitted for verification.</response>
    /// <response code="400">If the property is not in a submittable state.</response>
    /// <response code="403">If the user is not the owner.</response>
    /// <response code="404">If the property is not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpPut("{id:int}/submit")]
    public async Task<ActionResult<PropertyResponseDto>> SubmitForVerification(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _propertyService.SubmitForVerificationAsync(userId, id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a page of properties pending admin verification (status = Submitted). Admin only.
    /// </summary>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of properties awaiting verification.</returns>
    /// <response code="200">Returns the page of pending properties.</response>
    [Authorize(Roles = "Admin")]
    [HttpGet("pending-verification")]
    public async Task<ActionResult<PagedResultDto<PropertyResponseDto>>> GetPendingVerification([FromQuery] PaginationParams pagination)
    {
        var result = await _propertyService.GetPendingVerificationAsync(pagination);
        return Ok(result);
    }

    /// <summary>
    /// Approves or rejects a property verification request. Admin only.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <param name="approve">True to approve, false to reject.</param>
    /// <param name="dto">Optional remarks.</param>
    /// <returns>The updated property details.</returns>
    /// <response code="200">Property verification status updated.</response>
    /// <response code="400">If the property is not in Submitted state.</response>
    /// <response code="404">If the property is not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/verify")]
    public async Task<ActionResult<PropertyResponseDto>> VerifyProperty(int id, [FromQuery] bool approve, [FromBody] VerifyRequestDto dto)
    {
        var adminId = GetCurrentUserId();
        var result = await _propertyService.VerifyPropertyAsync(adminId, id, approve, dto);
        return Ok(result);
    }

    /// <summary>
    /// Uploads a property document PDF and returns a permanent URL. Owner only.
    /// </summary>
    /// <param name="file">The PDF file to upload (max 10 MB).</param>
    /// <returns>An object containing the URL of the stored document.</returns>
    /// <response code="200">File uploaded successfully; returns <c>{ "url": "..." }</c>.</response>
    /// <response code="400">If no file is provided, the file is not a PDF, or the file exceeds 10 MB.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not in the Owner role.</response>
    [Authorize(Roles = "Owner")]
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
        var uploadsDir = Path.Combine(webRoot, "uploads", "propertydocs");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}.pdf";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"{Request.Scheme}://{Request.Host}/uploads/propertydocs/{fileName}";
        return Ok(new { url });
    }

    /// <summary>
    /// Adds a document to a property. Owner only.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <param name="dto">The document details to associate.</param>
    /// <returns>The created document details.</returns>
    /// <response code="201">Document added successfully.</response>
    /// <response code="400">If the input is invalid.</response>
    /// <response code="403">If the user is not the property owner.</response>
    /// <response code="404">If the property is not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpPost("{id:int}/documents")]
    public async Task<ActionResult<DocumentResponseDto>> AddDocument(int id, [FromBody] AddPropertyDocumentDto dto)
    {
        var ownerId = GetCurrentUserId();
        var result = await _propertyService.AddDocumentAsync(ownerId, id, dto);
        return CreatedAtAction(nameof(GetDocuments), new { id }, result);
    }

    /// <summary>
    /// Retrieves a page of documents associated with a property.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A page of document details.</returns>
    /// <response code="200">Returns the page of documents.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="404">If the property is not found.</response>
    [Authorize]
    [HttpGet("{id:int}/documents")]
    public async Task<ActionResult<PagedResultDto<DocumentResponseDto>>> GetDocuments(int id, [FromQuery] PaginationParams pagination)
    {
        var result = await _propertyService.GetDocumentsAsync(id, pagination);
        return Ok(result);
    }

    /// <summary>
    /// Removes a document from a property. Owner only.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <param name="documentId">The unique identifier of the document to remove.</param>
    /// <returns>No Content response.</returns>
    /// <response code="204">Document removed successfully.</response>
    /// <response code="403">If the user is not the property owner.</response>
    /// <response code="404">If the property or document is not found.</response>
    [Authorize(Roles = "Owner")]
    [HttpDelete("{id:int}/documents/{documentId:guid}")]
    public async Task<IActionResult> RemoveDocument(int id, Guid documentId)
    {
        var ownerId = GetCurrentUserId();
        await _propertyService.RemoveDocumentAsync(ownerId, id, documentId);
        return NoContent();
    }

    /// <summary>
    /// Uploads one or more property images and returns their permanent URLs. Owner only.
    /// Accepted formats: JPEG, PNG, GIF, WebP. Maximum 5 MB per image.
    /// </summary>
    /// <param name="files">The image file(s) to upload.</param>
    /// <returns>An object containing the list of uploaded image URLs.</returns>
    /// <response code="200">Files uploaded successfully; returns <c>{ "urls": [...] }</c>.</response>
    /// <response code="400">If no files are provided, a file is not a supported image type, or a file exceeds 5 MB.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not in the Owner role.</response>
    [Authorize(Roles = "Owner")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = 50 * 1024 * 1024)]
    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImages(List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
            return BadRequest(new { detail = "No files were provided." });

        var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/gif", "image/webp"
        };

        const long maxFileSize = 5 * 1024 * 1024;

        foreach (var file in files)
        {
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest(new { detail = $"'{file.FileName}' is not a supported image type. Accepted: JPEG, PNG, GIF, WebP." });

            if (file.Length > maxFileSize)
                return BadRequest(new { detail = $"'{file.FileName}' exceeds the 5 MB size limit." });
        }

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadsDir = Path.Combine(webRoot, "uploads", "propertyimages");
        Directory.CreateDirectory(uploadsDir);

        var urls = new List<string>();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            urls.Add($"{Request.Scheme}://{Request.Host}/uploads/propertyimages/{fileName}");
        }

        return Ok(new { urls });
    }
}

