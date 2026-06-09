using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Property entity operations.
/// </summary>
public class PropertyRepository : IPropertyRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PropertyRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a property by its identifier.
    /// </summary>
    /// <param name="id">The property identifier.</param>
    /// <returns>The property entity if found.</returns>
    public async Task<Property?> GetByIdAsync(int id)
    {
        return await _context.Properties.FindAsync(id);
    }

    /// <summary>
    /// Retrieves all properties from the database.
    /// </summary>
    /// <returns>A collection of properties.</returns>
    public async Task<IEnumerable<Property>> GetAllAsync()
    {
        return await _context.Properties.ToListAsync();
    }

    /// <summary>
    /// Creates a new property.
    /// </summary>
    /// <param name="entity">The property to create.</param>
    /// <returns>The created property.</returns>
    public async Task<Property> CreateAsync(Property entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Properties.Add(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing property.
    /// </summary>
    /// <param name="entity">The property to update.</param>
    public async Task UpdateAsync(Property entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Properties.Update(entity);
    }

    /// <summary>
    /// Deletes a property by identifier.
    /// </summary>
    /// <param name="id">The property identifier.</param>
    public async Task DeleteAsync(int id)
    {
        var property = await GetByIdAsync(id);
        if (property != null)
        {
            _context.Properties.Remove(property);
        }
    }
}
