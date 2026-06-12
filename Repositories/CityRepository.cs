using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for City entity data operations.
/// </summary>
public class CityRepository : ICityRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CityRepository"/> class.
    /// </summary>
    /// <param name="context">The database context for data access.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public CityRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a city by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the city.</param>
    /// <returns>The city entity if found; otherwise null.</returns>
    public async Task<City?> GetByIdAsync(int id)
    {
        return await _context.Cities.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves all cities from the database that are not deleted.
    /// </summary>
    /// <returns>A collection of cities.</returns>
    public async Task<IEnumerable<City>> GetAllAsync()
    {
        return await _context.Cities.Where(c => c.DeletedAt == null).ToListAsync();
    }

    /// <summary>
    /// Creates a new city entity.
    /// </summary>
    /// <param name="entity">The city entity to create.</param>
    /// <returns>The created city entity.</returns>
    public async Task<City> CreateAsync(City entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Cities.Add(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing city entity.
    /// </summary>
    /// <param name="entity">The city entity to update.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public async Task UpdateAsync(City entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Cities.Update(entity);
    }

    /// <summary>
    /// Deletes a city by its identifier using soft delete.
    /// </summary>
    /// <param name="id">The unique identifier of the city to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public async Task DeleteAsync(int id)
    {
        var city = await GetByIdAsync(id);
        if (city != null)
        {
            city.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Cities.Update(city);
        }
    }
}
