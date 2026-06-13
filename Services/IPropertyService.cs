using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Retrieves a property by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <returns>A response DTO detailing the property.</returns>
    Task<PropertyResponseDto> GetPropertyByIdAsync(int id);

    /// <summary>
    /// Retrieves all property listings.
    /// </summary>
    /// <returns>A collection of property response DTOs.</returns>
    Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync();

    /// <summary>
    /// Updates an existing property listing. Requires ownership validation.
    /// </summary>
    /// <param name="userId">The unique identifier of the authenticated user performing the update.</param>
    /// <param name="id">The unique identifier of the property to update.</param>
    /// <param name="dto">The updated property details.</param>
    /// <returns>A response DTO detailing the updated property.</returns>
    Task<PropertyResponseDto> UpdatePropertyAsync(Guid userId, int id, UpdatePropertyDto dto);

    /// <summary>
    /// Deletes an existing property listing. Requires ownership validation.
    /// </summary>
    /// <param name="userId">The unique identifier of the authenticated user performing the deletion.</param>
    /// <param name="id">The unique identifier of the property to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeletePropertyAsync(Guid userId, int id);

    /// <summary>
    /// Retrieves all properties owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <returns>A collection of property response DTOs.</returns>
    Task<IEnumerable<PropertyResponseDto>> GetPropertiesByOwnerIdAsync(Guid ownerId);
}

