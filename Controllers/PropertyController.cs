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

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyController"/> class.
    /// </summary>
    /// <param name="propertyService">The property service.</param>
    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
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
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <returns>The property details.</returns>
    /// <response code="200">Returns the property details.</response>
    /// <response code="404">If the property is not found.</response>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropertyResponseDto>> GetPropertyById(int id)
    {
        var result = await _propertyService.GetPropertyByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all property listings.
    /// </summary>
    /// <returns>A list of property listings.</returns>
    /// <response code="200">Returns the list of properties.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetAllProperties()
    {
        var result = await _propertyService.GetAllPropertiesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all property listings owned by the currently authenticated owner.
    /// </summary>
    /// <returns>A list of property listings owned by the user.</returns>
    /// <response code="200">Returns the list of owner's properties.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not in the Owner role.</response>
    [Authorize(Roles = "Owner")]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<PropertyResponseDto>>> GetMyProperties()
    {
        var userId = GetCurrentUserId();
        var result = await _propertyService.GetPropertiesByOwnerIdAsync(userId);
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
}

