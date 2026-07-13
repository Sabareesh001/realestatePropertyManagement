using System;
using System.Collections.Generic;
using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Lease entity operations.
/// </summary>
public interface ILeaseRepository : IRepository<Lease, Guid>
{
    /// <summary>
    /// Retrieves a lease by its identifier, eager loading its associated documents.
    /// </summary>
    /// <param name="id">The lease identifier.</param>
    /// <returns>The lease entity with documents loaded if found; otherwise null.</returns>
    Task<Lease?> GetByIdWithDocumentsAsync(Guid id);

    /// <summary>
    /// Retrieves a page of leases in Submitted status whose templates are awaiting admin verification.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of leases pending template verification.</returns>
    Task<PagedResultDto<Lease>> GetPendingTemplatesAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a page of leases in TenantSigned status whose signed agreements are awaiting admin verification.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of leases pending signed agreement verification.</returns>
    Task<PagedResultDto<Lease>> GetPendingSignedLeasesAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Returns true when a non-terminal lease already exists for the given proposal.
    /// Terminal statuses are Rejected, Terminated, and Expired.
    /// </summary>
    /// <param name="proposalId">The proposal identifier to check.</param>
    /// <returns>True if an active-pipeline lease exists for the proposal; otherwise false.</returns>
    Task<bool> ExistsByProposalIdAsync(Guid proposalId);

    /// <summary>
    /// Returns true when a non-terminal lease on the given property overlaps the specified date range.
    /// </summary>
    /// <param name="propertyId">The property identifier to check.</param>
    /// <param name="startDate">The proposed start date.</param>
    /// <param name="endDate">The proposed end date.</param>
    /// <param name="excludeLeaseId">Optional lease identifier to exclude from the check (used during updates).</param>
    /// <returns>True if an overlapping lease exists; otherwise false.</returns>
    Task<bool> HasOverlappingLeaseAsync(int propertyId, DateOnly startDate, DateOnly endDate, Guid? excludeLeaseId = null);
}
