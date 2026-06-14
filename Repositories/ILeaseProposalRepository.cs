using System;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for LeaseProposal entity operations.
/// </summary>
public interface ILeaseProposalRepository : IRepository<LeaseProposal, Guid>
{
    /// <summary>
    /// Retrieves all lease proposals submitted by a specific tenant, eager loading property information.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A collection of lease proposals.</returns>
    Task<IEnumerable<LeaseProposal>> GetProposalsByTenantIdAsync(Guid tenantId);

    /// <summary>
    /// Retrieves all lease proposals received for properties owned by a specific owner, eager loading tenant user details.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <returns>A collection of lease proposals.</returns>
    Task<IEnumerable<LeaseProposal>> GetProposalsByOwnerIdAsync(Guid ownerId);

    /// <summary>
    /// Checks if there is any overlapping lease proposal for a given property that is not rejected, cancelled, or expired.
    /// </summary>
    /// <param name="propertyId">The unique identifier of the property.</param>
    /// <param name="startDate">The proposed lease start date.</param>
    /// <param name="endDate">The proposed lease end date.</param>
    /// <returns>True if an overlapping active proposal exists; otherwise false.</returns>
    Task<bool> HasOverlappingProposalAsync(int propertyId, DateOnly startDate, DateOnly endDate);
}

