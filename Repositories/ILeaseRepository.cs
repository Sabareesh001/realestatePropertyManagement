using System;
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
}
