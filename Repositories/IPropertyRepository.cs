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
    /// Retrieves all properties owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <returns>A collection of properties.</returns>
    Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(Guid ownerId);

    /// <summary>
    /// Retrieves all properties pending admin verification (status = Submitted).
    /// </summary>
    /// <returns>A collection of properties awaiting verification.</returns>
    Task<IEnumerable<Property>> GetPendingVerificationAsync();

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

