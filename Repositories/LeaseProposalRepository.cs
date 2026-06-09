using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
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
        return await _context.LeaseProposals.FindAsync(id);
    }

    /// <summary>
    /// Retrieves all lease proposals from the database.
    /// </summary>
    /// <returns>A collection of lease proposals.</returns>
    public async Task<IEnumerable<LeaseProposal>> GetAllAsync()
    {
        return await _context.LeaseProposals.ToListAsync();
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
            _context.LeaseProposals.Remove(proposal);
        }
    }
}
