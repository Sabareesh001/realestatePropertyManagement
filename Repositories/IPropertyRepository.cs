using propertyManagement.Models;
using System;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Property entity operations.
/// </summary>
public interface IPropertyRepository : IRepository<Property, int>
{
}
