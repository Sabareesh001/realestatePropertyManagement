using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Payment entity operations.
/// </summary>
public interface IPaymentRepository : IRepository<Payment, Guid>
{
    /// <summary>
    /// Retrieves all payments associated with a specific lease via charge-payment relationships.
    /// </summary>
    /// <param name="leaseId">The lease identifier.</param>
    /// <returns>A collection of payments linked to the specified lease.</returns>
    Task<IEnumerable<Payment>> GetByLeaseIdAsync(Guid leaseId);
}
