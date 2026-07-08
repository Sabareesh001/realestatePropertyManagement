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
    /// Non-owners receive a "not found" response for Draft, Submitted, or Rejected properties.
    /// </summary>
    /// <param name="id">The unique identifier of the property.</param>
    /// <param name="requestingUserId">The ID of the authenticated requester, or <c>null</c> for anonymous callers.</param>
    /// <returns>A response DTO detailing the property.</returns>
    Task<PropertyResponseDto> GetPropertyByIdAsync(int id, Guid? requestingUserId = null);

    /// <summary>
    /// Retrieves all Verified property listings visible to the public.
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

    /// <summary>
    /// Submits a Draft property for admin verification. Only the owner can submit.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the authenticated owner.</param>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <returns>The updated property response DTO.</returns>
    Task<PropertyResponseDto> SubmitForVerificationAsync(Guid ownerId, int propertyId);

    /// <summary>
    /// Retrieves all properties pending admin verification.
    /// </summary>
    /// <returns>A collection of property response DTOs with Submitted status.</returns>
    Task<IEnumerable<PropertyResponseDto>> GetPendingVerificationAsync();

    /// <summary>
    /// Approves or rejects a property verification request. Admin only.
    /// </summary>
    /// <param name="adminId">The unique identifier of the admin performing the action.</param>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <param name="approve">True to approve, false to reject.</param>
    /// <param name="dto">Optional remarks DTO.</param>
    /// <returns>The updated property response DTO.</returns>
    Task<PropertyResponseDto> VerifyPropertyAsync(Guid adminId, int propertyId, bool approve, VerifyRequestDto dto);

    /// <summary>
    /// Adds a document to a property. Requires ownership.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the authenticated owner.</param>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <param name="dto">The document details to associate.</param>
    /// <returns>The created document response DTO.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the property is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller is not the property owner.</exception>
    Task<DocumentResponseDto> AddDocumentAsync(Guid ownerId, int propertyId, AddPropertyDocumentDto dto);

    /// <summary>
    /// Retrieves all active documents associated with a property.
    /// </summary>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <returns>A collection of document response DTOs.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the property is not found.</exception>
    Task<IEnumerable<DocumentResponseDto>> GetDocumentsAsync(int propertyId);

    /// <summary>
    /// Removes a document from a property and soft-deletes the document record. Requires ownership.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the authenticated owner.</param>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <param name="documentId">The unique identifier of the document to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the property or document is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller is not the property owner.</exception>
    Task RemoveDocumentAsync(Guid ownerId, int propertyId, Guid documentId);
}

