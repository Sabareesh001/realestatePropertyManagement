using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Lease entity operations.
/// </summary>
public class LeaseRepository : ILeaseRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public LeaseRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a lease by its identifier, eager loading related entities.
    /// </summary>
    /// <param name="id">The lease identifier.</param>
    /// <returns>The lease entity if found.</returns>
    public async Task<Lease?> GetByIdAsync(Guid id)
    {
        return await _context.Leases
            .Include(l => l.Tenant)
            .Include(l => l.PropertyNavigation)
            .Include(l => l.Status)
            .Include(l => l.AgreementDocument)
            .Include(l => l.SignedAgreementDocument)
            .FirstOrDefaultAsync(l => l.Id == id && l.DeletedAt == null && (l.PropertyNavigation == null || l.PropertyNavigation.DeletedAt == null));
    }

    /// <summary>
    /// Retrieves all leases, eager loading related entities.
    /// </summary>
    /// <returns>A collection of leases.</returns>
    public async Task<IEnumerable<Lease>> GetAllAsync()
    {
        return await _context.Leases
            .Include(l => l.Tenant)
            .Include(l => l.PropertyNavigation)
            .Include(l => l.Status)
            .Include(l => l.AgreementDocument)
            .Include(l => l.SignedAgreementDocument)
            .Where(l => l.DeletedAt == null && (l.PropertyNavigation == null || l.PropertyNavigation.DeletedAt == null))
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new lease record.
    /// </summary>
    /// <param name="entity">The lease to create.</param>
    /// <returns>The created lease.</returns>
    public async Task<Lease> CreateAsync(Lease entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        await _context.Leases.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing lease record.
    /// </summary>
    /// <param name="entity">The lease to update.</param>
    public async Task UpdateAsync(Lease entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Leases.Update(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a lease record by its identifier.
    /// </summary>
    /// <param name="id">The lease identifier.</param>
    public async Task DeleteAsync(Guid id)
    {
        var lease = await _context.Leases.FirstOrDefaultAsync(l => l.Id == id && l.DeletedAt == null);
        if (lease != null)
        {
            lease.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Leases.Update(lease);
        }
    }
}
