using System;
using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for LeaseProposal entity operations.
/// </summary>
public interface ILeaseProposalRepository : IRepository<LeaseProposal, Guid>
{
    /// <summary>
    /// Retrieves a page of lease proposals submitted by a specific tenant, eager loading property information.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of lease proposals.</returns>
    Task<PagedResultDto<LeaseProposal>> GetProposalsByTenantIdAsync(Guid tenantId, int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a page of lease proposals received for properties owned by a specific owner, eager loading tenant user details.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of lease proposals.</returns>
    Task<PagedResultDto<LeaseProposal>> GetProposalsByOwnerIdAsync(Guid ownerId, int pageNumber, int pageSize);

    /// <summary>
    /// Checks if there is any overlapping lease proposal for a given property that is not rejected, cancelled, or expired.
    /// </summary>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <param name="startDate">The proposed lease start date.</param>
    /// <param name="endDate">The proposed lease end date.</param>
    /// <returns>True if an overlapping active proposal exists; otherwise false.</returns>
    Task<bool> HasOverlappingProposalAsync(int propertyId, DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Returns true when the tenant already has a lease proposal in Draft or Submitted status.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>True if the tenant has a pending proposal; otherwise false.</returns>
    Task<bool> HasActivePendingProposalAsync(Guid tenantId);
}

