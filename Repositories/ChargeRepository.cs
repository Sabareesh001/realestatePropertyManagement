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
/// Repository implementation for Charge entity operations.
/// </summary>
public class ChargeRepository : IChargeRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChargeRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ChargeRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a charge by its identifier, eager loading related entities.
    /// </summary>
    /// <param name="id">The charge identifier.</param>
    /// <returns>The charge entity if found; otherwise null.</returns>
    public async Task<Charge?> GetByIdAsync(Guid id)
    {
        return await _context.Charges
            .Include(c => c.ChargeType)
            .Include(c => c.Status)
            .Include(c => c.ChargePayments)
                .ThenInclude(cp => cp.Payment)
            .FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves all charges, eager loading related entities.
    /// </summary>
    /// <returns>A collection of all charges.</returns>
    public async Task<IEnumerable<Charge>> GetAllAsync()
    {
        return await _context.Charges
            .Include(c => c.ChargeType)
            .Include(c => c.Status)
            .Where(c => c.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all charges associated with a specific lease.
    /// </summary>
    /// <param name="leaseId">The lease identifier.</param>
    /// <returns>A collection of charges for the specified lease.</returns>
    public async Task<PagedResultDto<Charge>> GetByLeaseIdAsync(Guid leaseId, int pageNumber, int pageSize)
    {
        return await _context.Charges
            .Include(c => c.ChargeType)
            .Include(c => c.Status)
            .Include(c => c.ChargePayments)
                .ThenInclude(cp => cp.Payment)
            .Where(c => c.DeletedAt == null && c.Leases.Any(l => l.Id == leaseId))
            .OrderByDescending(c => c.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Retrieves a charge by its identifier, eager loading payment details.
    /// </summary>
    /// <param name="chargeId">The charge identifier.</param>
    /// <returns>The charge entity with payment details if found; otherwise null.</returns>
    public async Task<Charge?> GetByIdWithPaymentsAsync(Guid chargeId)
    {
        return await _context.Charges
            .Include(c => c.ChargeType)
            .Include(c => c.Status)
            .Include(c => c.Leases)
            .Include(c => c.ChargePayments)
                .ThenInclude(cp => cp.Payment)
                    .ThenInclude(p => p.PaymentMethod)
            .Include(c => c.ChargePayments)
                .ThenInclude(cp => cp.Payment)
                    .ThenInclude(p => p.Status)
            .FirstOrDefaultAsync(c => c.Id == chargeId && c.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves every charge across all leases for the admin dashboard, eager loading lease,
    /// property, owner and tenant context. Results are ordered newest first.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound (UTC) on the charge creation date.</param>
    /// <param name="to">Optional inclusive upper bound (UTC) on the charge creation date.</param>
    /// <returns>A collection of charges with full context.</returns>
    public async Task<IEnumerable<Charge>> GetAllForAdminAsync(DateTime? from, DateTime? to)
    {
        var fromDate = from.HasValue ? DateTime.SpecifyKind(from.Value, DateTimeKind.Unspecified) : (DateTime?)null;
        var toDate = to.HasValue ? DateTime.SpecifyKind(to.Value, DateTimeKind.Unspecified) : (DateTime?)null;

        return await _context.Charges
            .Include(c => c.ChargeType)
            .Include(c => c.Status)
            .Include(c => c.ChargePayments)
                .ThenInclude(cp => cp.Payment)
            .Include(c => c.Leases)
                .ThenInclude(l => l.PropertyNavigation)
                    .ThenInclude(pr => pr!.Owner)
            .Include(c => c.Leases)
                .ThenInclude(l => l.Tenant)
            .Where(c => c.DeletedAt == null
                        && (fromDate == null || c.CreatedAt >= fromDate)
                        && (toDate == null || c.CreatedAt <= toDate))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a page of charges across all leases for the admin dashboard, eager loading lease,
    /// property, owner and tenant context. Results are ordered newest first.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound (UTC) on the charge creation date.</param>
    /// <param name="to">Optional inclusive upper bound (UTC) on the charge creation date.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of charges with full context.</returns>
    public async Task<PagedResultDto<Charge>> GetAllForAdminAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize)
    {
        var fromDate = from.HasValue ? DateTime.SpecifyKind(from.Value, DateTimeKind.Unspecified) : (DateTime?)null;
        var toDate = to.HasValue ? DateTime.SpecifyKind(to.Value, DateTimeKind.Unspecified) : (DateTime?)null;

        return await _context.Charges
            .Include(c => c.ChargeType)
            .Include(c => c.Status)
            .Include(c => c.ChargePayments)
                .ThenInclude(cp => cp.Payment)
            .Include(c => c.Leases)
                .ThenInclude(l => l.PropertyNavigation)
                    .ThenInclude(pr => pr!.Owner)
            .Include(c => c.Leases)
                .ThenInclude(l => l.Tenant)
            .Where(c => c.DeletedAt == null
                        && (fromDate == null || c.CreatedAt >= fromDate)
                        && (toDate == null || c.CreatedAt <= toDate))
            .OrderByDescending(c => c.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Creates a new charge record.
    /// </summary>
    /// <param name="entity">The charge to create.</param>
    /// <returns>The created charge.</returns>
    public async Task<Charge> CreateAsync(Charge entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        await _context.Charges.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing charge record.
    /// </summary>
    /// <param name="entity">The charge to update.</param>
    public async Task UpdateAsync(Charge entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Charges.Update(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Soft deletes a charge record by its identifier.
    /// </summary>
    /// <param name="id">The charge identifier.</param>
    public async Task DeleteAsync(Guid id)
    {
        var charge = await _context.Charges.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
        if (charge != null)
        {
            charge.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Charges.Update(charge);
        }
    }
}
