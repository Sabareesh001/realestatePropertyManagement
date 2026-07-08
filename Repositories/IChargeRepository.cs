using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Charge entity operations.
/// </summary>
public interface IChargeRepository : IRepository<Charge, Guid>
{
    /// <summary>
    /// Retrieves all charges associated with a specific lease.
    /// </summary>
    /// <param name="leaseId">The lease identifier.</param>
    /// <returns>A collection of charges for the specified lease.</returns>
    Task<IEnumerable<Charge>> GetByLeaseIdAsync(Guid leaseId);

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
}
