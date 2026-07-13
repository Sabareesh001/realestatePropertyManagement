using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Charge entity operations.
/// </summary>
public interface IChargeRepository : IRepository<Charge, Guid>
{
    /// <summary>
    /// Retrieves a page of charges associated with a specific lease.
    /// </summary>
    /// <param name="leaseId">The lease identifier.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of charges for the specified lease.</returns>
    Task<PagedResultDto<Charge>> GetByLeaseIdAsync(Guid leaseId, int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a charge by its identifier, eager loading payment details.
    /// </summary>
    /// <param name="chargeId">The charge identifier.</param>
    /// <returns>The charge entity with payment details if found; otherwise null.</returns>
    Task<Charge?> GetByIdWithPaymentsAsync(Guid chargeId);

    /// <summary>
    /// Retrieves every charge across all leases for the admin dashboard, eager loading lease,
    /// property, owner and tenant context. Results are ordered newest first.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound (UTC) on the charge creation date.</param>
    /// <param name="to">Optional inclusive upper bound (UTC) on the charge creation date.</param>
    /// <returns>A collection of charges with full context.</returns>
    Task<IEnumerable<Charge>> GetAllForAdminAsync(DateTime? from, DateTime? to);

    /// <summary>
    /// Retrieves a page of charges across all leases for the admin dashboard, eager loading lease,
    /// property, owner and tenant context. Results are ordered newest first.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound (UTC) on the charge creation date.</param>
    /// <param name="to">Optional inclusive upper bound (UTC) on the charge creation date.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of charges with full context.</returns>
    Task<PagedResultDto<Charge>> GetAllForAdminAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
}
