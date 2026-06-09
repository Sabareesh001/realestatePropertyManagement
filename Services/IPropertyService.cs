using System;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for property-related business operations.
/// </summary>
public interface IPropertyService
{
    /// <summary>
    /// Creates/posts a new property. Requires the user to be verified.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner posting the property.</param>
    /// <param name="dto">The property creation details.</param>
    /// <returns>A response DTO detailing the created property.</returns>
    Task<PropertyResponseDto> CreatePropertyAsync(Guid ownerId, CreatePropertyDto dto);
}
