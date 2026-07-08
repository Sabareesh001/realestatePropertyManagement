using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for ComplaintComment entity data operations.
/// </summary>
public class ComplaintCommentRepository : IComplaintCommentRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="ComplaintCommentRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ComplaintCommentRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<ComplaintComment?> GetByIdAsync(Guid id)
    {
        return await _context.ComplaintComments
            .Include(c => c.Author)
                .ThenInclude(a => a.UserRoles.Where(ur => ur.DeletedAt == null))
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ComplaintComment>> GetAllAsync()
    {
        return await _context.ComplaintComments.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ComplaintComment>> GetByComplaintIdAsync(Guid complaintId)
    {
        return await _context.ComplaintComments
            .Include(c => c.Author)
                .ThenInclude(a => a.UserRoles.Where(ur => ur.DeletedAt == null))
                    .ThenInclude(ur => ur.Role)
            .Where(c => c.ComplaintId == complaintId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<ComplaintComment> CreateAsync(ComplaintComment entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.ComplaintComments.Add(entity);
        return entity;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(ComplaintComment entity)
    {
        _context.ComplaintComments.Update(entity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        var comment = await GetByIdAsync(id);
        if (comment != null)
            _context.ComplaintComments.Remove(comment);
    }
}
