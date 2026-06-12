using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;
using propertyManagement.Filters;

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
    [AuthorizeRoles("Owner")]
    [HttpPost]
    public async Task<ActionResult<PropertyResponseDto>> CreateProperty([FromBody] CreatePropertyDto dto)
    {
        var userId = GetCurrentUserId();
        var result = await _propertyService.CreatePropertyAsync(userId, dto);
        return CreatedAtAction(null, result);
    }

}
