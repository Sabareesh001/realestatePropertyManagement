using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Property entity operations.
/// </summary>
public class PropertyRepository : IPropertyRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PropertyRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a property by its identifier.
    /// </summary>
    /// <param name="id">The property identifier.</param>
    /// <returns>The property entity if found.</returns>
    public async Task<Property?> GetByIdAsync(int id)
    {
        return await _context.Properties
            .Include(p => p.PropertyImages.Where(pi => pi.DeletedAt == null))
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves all properties from the database.
    /// </summary>
    /// <returns>A collection of properties.</returns>
    public async Task<IEnumerable<Property>> GetAllAsync()
    {
        return await _context.Properties
            .Include(p => p.PropertyImages.Where(pi => pi.DeletedAt == null))
            .Where(p => p.DeletedAt == null && p.VerificationStatusId == PropertyVerificationStatus.Verified)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a page of Verified properties visible to the public.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of properties.</returns>
    public async Task<PagedResultDto<Property>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _context.Properties
            .Include(p => p.PropertyImages.Where(pi => pi.DeletedAt == null))
            .Where(p => p.DeletedAt == null && p.VerificationStatusId == PropertyVerificationStatus.Verified)
            .OrderByDescending(p => p.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Creates a new property.
    /// </summary>
    /// <param name="entity">The property to create.</param>
    /// <returns>The created property.</returns>
    public async Task<Property> CreateAsync(Property entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Properties.Add(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing property.
    /// </summary>
    /// <param name="entity">The property to update.</param>
    public async Task UpdateAsync(Property entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Properties.Update(entity);
    }

    /// <summary>
    /// Deletes a property by identifier.
    /// </summary>
    /// <param name="id">The property identifier.</param>
    public async Task DeleteAsync(int id)
    {
        var property = await GetByIdAsync(id);
        if (property != null)
        {
            property.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Properties.Update(property);
        }
    }

    /// <summary>
    /// Retrieves a page of properties owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of properties.</returns>
    public async Task<PagedResultDto<Property>> GetPropertiesByOwnerIdAsync(Guid ownerId, int pageNumber, int pageSize)
    {
        return await _context.Properties
            .Include(p => p.PropertyImages.Where(pi => pi.DeletedAt == null))
            .Where(p => p.OwnerId == ownerId && p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Retrieves a page of properties pending admin verification (status = Submitted).
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of properties awaiting verification.</returns>
    public async Task<PagedResultDto<Property>> GetPendingVerificationAsync(int pageNumber, int pageSize)
    {
        return await _context.Properties
            .Include(p => p.PropertyImages.Where(pi => pi.DeletedAt == null))
            .Where(p => p.VerificationStatusId == PropertyVerificationStatus.Submitted && p.DeletedAt == null)
            .OrderBy(p => p.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Retrieves a property by identifier, including its active images and active documents.
    /// </summary>
    /// <param name="id">The property identifier.</param>
    /// <returns>The property entity with documents loaded, or <c>null</c> if not found.</returns>
    public async Task<Property?> GetByIdWithDocumentsAsync(int id)
    {
        return await _context.Properties
            .Include(p => p.PropertyImages.Where(pi => pi.DeletedAt == null))
            .Include(p => p.Documents.Where(d => d.DeletedAt == null))
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
    }

    /// <summary>
    /// Inserts a row into the property_documents join table linking the given property and document.
    /// </summary>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="documentId">The document identifier.</param>
    public async Task LinkDocumentAsync(int propertyId, Guid documentId)
    {
        var joinEntry = new Dictionary<string, object>
        {
            { "PropertyId", propertyId },
            { "DocumentId", documentId }
        };
        await _context.Set<Dictionary<string, object>>("PropertyDocument").AddAsync(joinEntry);
    }

    /// <summary>
    /// Removes the row from the property_documents join table for the given property and document.
    /// </summary>
    /// <param name="propertyId">The property identifier.</param>
    /// <param name="documentId">The document identifier.</param>
    public async Task UnlinkDocumentAsync(int propertyId, Guid documentId)
    {
        var joinSet = _context.Set<Dictionary<string, object>>("PropertyDocument");
        var entry = await joinSet.FindAsync(propertyId, documentId);
        if (entry != null)
            joinSet.Remove(entry);
    }
}
