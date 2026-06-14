using System;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Lease entity operations.
/// </summary>
public interface ILeaseRepository : IRepository<Lease, Guid>
{
}
