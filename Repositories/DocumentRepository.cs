using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Document entity operations.
/// </summary>
public class DocumentRepository : IDocumentRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DocumentRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Creates a new document record.
    /// </summary>
    /// <param name="entity">The document to create.</param>
    /// <returns>The created document.</returns>
    public async Task<Document> CreateAsync(Document entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        await _context.Documents.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Retrieves a document by its identifier.
    /// </summary>
    /// <param name="id">The document identifier.</param>
    /// <returns>The document entity if found; otherwise null.</returns>
    public async Task<Document?> GetByIdAsync(Guid id)
    {
        return await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves all active documents.
    /// </summary>
    /// <returns>A collection of documents.</returns>
    public async Task<IEnumerable<Document>> GetAllAsync()
    {
        return await _context.Documents.Where(d => d.DeletedAt == null).ToListAsync();
    }

    /// <summary>
    /// Updates an existing document record.
    /// </summary>
    /// <param name="entity">The document to update.</param>
    public async Task UpdateAsync(Document entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Documents.Update(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a document record by its identifier (soft delete).
    /// </summary>
    /// <param name="id">The document identifier.</param>
    public async Task DeleteAsync(Guid id)
    {
        var doc = await _context.Documents.FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);
        if (doc != null)
        {
            doc.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Documents.Update(doc);
        }
    }
}
