using propertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Property entity operations.
/// </summary>
public interface IPropertyRepository : IRepository<Property, int>
{
    /// <summary>
    /// Retrieves all properties owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <returns>A collection of properties.</returns>
    Task<IEnumerable<Property>> GetPropertiesByOwnerIdAsync(Guid ownerId);
}

