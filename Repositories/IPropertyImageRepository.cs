using propertyManagement.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for PropertyImage entity operations.
/// </summary>
public interface IPropertyImageRepository : IRepository<PropertyImage, Guid>
{
    /// <summary>
    /// Retrieves all images associated with a specific property.
    /// </summary>
    /// <param name="propertyId">The identifier of the property.</param>
    /// <returns>A collection of property images.</returns>
    Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId);
}
