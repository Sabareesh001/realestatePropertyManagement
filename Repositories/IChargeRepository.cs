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
}
