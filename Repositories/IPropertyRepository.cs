using propertyManagement.DTOs;
using propertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Property entity operations.
/// </summary>
public interface IPropertyRepository : IRepository<Property, int>
{
    /// <summary>
    /// Retrieves a page of Verified properties visible to the public.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of properties.</returns>
    Task<PagedResultDto<Property>> GetAllAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a page of properties owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of properties.</returns>
    Task<PagedResultDto<Property>> GetPropertiesByOwnerIdAsync(Guid ownerId, int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a page of properties pending admin verification (status = Submitted).
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of properties awaiting verification.</returns>
    Task<PagedResultDto<Property>> GetPendingVerificationAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a property by identifier, including its active images and active documents.
    /// </summary>
    /// <param name="id">The property identifier.</param>
    /// <returns>The property entity with documents loaded, or <c>null</c> if not found.</returns>
    Task<Property?> GetByIdWithDocumentsAsync(int id);

    /// <summary>
    /// Inserts a row into the property_documents join table linking the given property and document.
    /// </summary>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="documentId">The document identifier.</param>
    Task LinkDocumentAsync(int propertyId, Guid documentId);

    /// <summary>
    /// Removes the row from the property_documents join table for the given property and document.
    /// </summary>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="documentId">The document identifier.</param>
    Task UnlinkDocumentAsync(int propertyId, Guid documentId);
}

