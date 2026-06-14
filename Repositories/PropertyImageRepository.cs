using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for PropertyImage entity operations.
/// </summary>
public class PropertyImageRepository : IPropertyImageRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyImageRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PropertyImageRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<PropertyImage> CreateAsync(PropertyImage entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        var result = await _context.PropertyImages.AddAsync(entity);
        return result.Entity;
    }

    /// <inheritdoc />
    public async Task<PropertyImage?> GetByIdAsync(Guid id)
    {
        return await _context.PropertyImages.FirstOrDefaultAsync(pi => pi.Id == id && pi.DeletedAt == null);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PropertyImage>> GetAllAsync()
    {
        return await _context.PropertyImages.Where(pi => pi.DeletedAt == null).ToListAsync();
    }

    /// <inheritdoc />
    public Task UpdateAsync(PropertyImage entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.PropertyImages.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var propertyImage = await GetByIdAsync(id);
        if (propertyImage != null)
        {
            propertyImage.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.PropertyImages.Update(propertyImage);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PropertyImage>> GetByPropertyIdAsync(int propertyId)
    {
        return await _context.PropertyImages
            .Where(pi => pi.PropertyId == propertyId && pi.DeletedAt == null)
            .OrderBy(pi => pi.DisplayOrder)
            .ToListAsync();
    }
}
