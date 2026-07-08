using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Complaint entity data operations.
/// </summary>
public class ComplaintRepository : IComplaintRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="ComplaintRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ComplaintRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Complaint?> GetByIdAsync(Guid id)
    {
        return await _context.Complaints
            .Include(c => c.ComplaintType)
            .Include(c => c.Priority)
            .Include(c => c.Status)
            .Include(c => c.Property)
            .Include(c => c.Tenant)
            .Include(c => c.Comments.OrderBy(cm => cm.CreatedAt))
                .ThenInclude(cm => cm.Author)
                    .ThenInclude(a => a.UserRoles.Where(ur => ur.DeletedAt == null))
                        .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Complaint>> GetAllAsync()
    {
        return await GetAllWithDetailsAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Complaint>> GetAllWithDetailsAsync()
    {
        return await _context.Complaints
            .Include(c => c.ComplaintType)
            .Include(c => c.Priority)
            .Include(c => c.Status)
            .Include(c => c.Property)
            .Include(c => c.Tenant)
            .Include(c => c.Comments)
            .Where(c => c.DeletedAt == null)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Complaint>> GetByCreatedByAsync(Guid userId)
    {
        return await _context.Complaints
            .Include(c => c.ComplaintType)
            .Include(c => c.Priority)
            .Include(c => c.Status)
            .Include(c => c.Property)
            .Include(c => c.Tenant)
            .Include(c => c.Comments)
            .Where(c => c.CreatedBy == userId && c.DeletedAt == null)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Complaint>> GetByOwnerIdAsync(Guid ownerId)
    {
        return await _context.Complaints
            .Include(c => c.ComplaintType)
            .Include(c => c.Priority)
            .Include(c => c.Status)
            .Include(c => c.Property)
            .Include(c => c.Tenant)
            .Include(c => c.Comments)
            .Where(c => c.OwnerId == ownerId && c.DeletedAt == null)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Complaint> CreateAsync(Complaint entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Complaints.Add(entity);
        return entity;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Complaint entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Complaints.Update(entity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        var complaint = await GetByIdAsync(id);
        if (complaint != null)
        {
            complaint.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Complaints.Update(complaint);
        }
    }
}
