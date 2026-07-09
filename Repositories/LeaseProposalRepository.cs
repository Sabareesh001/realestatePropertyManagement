using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for LeaseProposal entity operations.
/// </summary>
public class LeaseProposalRepository : ILeaseProposalRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseProposalRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public LeaseProposalRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a lease proposal by its identifier.
    /// </summary>
    /// <param name="id">The lease proposal identifier.</param>
    /// <returns>The lease proposal entity if found.</returns>
    public async Task<LeaseProposal?> GetByIdAsync(Guid id)
    {
        return await _context.LeaseProposals.FirstOrDefaultAsync(lp => lp.Id == id && lp.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves all lease proposals from the database.
    /// </summary>
    /// <returns>A collection of lease proposals.</returns>
    public async Task<IEnumerable<LeaseProposal>> GetAllAsync()
    {
        return await _context.LeaseProposals.Where(lp => lp.DeletedAt == null).ToListAsync();
    }

    /// <summary>
    /// Creates a new lease proposal.
    /// </summary>
    /// <param name="entity">The lease proposal to create.</param>
    /// <returns>The created lease proposal.</returns>
    public async Task<LeaseProposal> CreateAsync(LeaseProposal entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.LeaseProposals.Add(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing lease proposal.
    /// </summary>
    /// <param name="entity">The lease proposal to update.</param>
    public async Task UpdateAsync(LeaseProposal entity)
    {
        _context.LeaseProposals.Update(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a lease proposal by identifier.
    /// </summary>
    /// <param name="id">The lease proposal identifier.</param>
    public async Task DeleteAsync(Guid id)
    {
        var proposal = await GetByIdAsync(id);
        if (proposal != null)
        {
            proposal.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.LeaseProposals.Update(proposal);
        }
    }

    /// <summary>
    /// Retrieves all lease proposals submitted by a specific tenant, eager loading property information.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A collection of lease proposals.</returns>
    public async Task<PagedResultDto<LeaseProposal>> GetProposalsByTenantIdAsync(Guid tenantId, int pageNumber, int pageSize)
    {
        return await _context.LeaseProposals
            .Include(p => p.Property)
            .Include(p => p.Status)
            .Where(p => p.TenantId == tenantId && p.DeletedAt == null && (p.Property == null || p.Property.DeletedAt == null))
            .OrderByDescending(p => p.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Retrieves all lease proposals received for properties owned by a specific owner, eager loading tenant details.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <returns>A collection of lease proposals.</returns>
    public async Task<PagedResultDto<LeaseProposal>> GetProposalsByOwnerIdAsync(Guid ownerId, int pageNumber, int pageSize)
    {
        return await _context.LeaseProposals
            .Include(p => p.Property)
            .Include(p => p.Status)
            .Include(p => p.Tenant)
                .ThenInclude(t => t!.UserProfile)
                    .ThenInclude(up => up!.TenantProfile)
            .Where(p => p.Property != null && p.Property.OwnerId == ownerId && p.DeletedAt == null && p.Property.DeletedAt == null && p.StatusId != ProposalStatus.Draft)
            .OrderByDescending(p => p.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <summary>
    /// Checks if there is an approved lease proposal for a given property that overlaps with the specified date range.
    /// Only an approved proposal blocks new proposals, since multiple tenants should be able to compete
    /// (submit proposals) for the same property simultaneously. Once a proposal is approved, the property
    /// is considered spoken for and no further overlapping proposals are accepted.
    /// </summary>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <param name="startDate">The proposed lease start date.</param>
    /// <param name="endDate">The proposed lease end date.</param>
    /// <returns>True if an approved overlapping proposal exists; otherwise false.</returns>
    public async Task<bool> HasOverlappingProposalAsync(int propertyId, DateOnly startDate, DateOnly endDate)
    {
        return await _context.LeaseProposals
            .AnyAsync(lp => lp.PropertyId == propertyId &&
                            lp.DeletedAt == null &&
                            lp.StatusId == ProposalStatus.Approved &&
                            lp.StartDate <= endDate &&
                            lp.EndDate >= startDate);
    }

    /// <summary>
    /// Returns true when the tenant already has a lease proposal in Draft or Submitted status.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>True if the tenant has a pending proposal; otherwise false.</returns>
    public async Task<bool> HasActivePendingProposalAsync(Guid tenantId)
    {
        return await _context.LeaseProposals
            .AnyAsync(lp => lp.TenantId == tenantId &&
                            lp.DeletedAt == null &&
                            (lp.StatusId == ProposalStatus.Draft || lp.StatusId == ProposalStatus.Submitted));
    }
}

