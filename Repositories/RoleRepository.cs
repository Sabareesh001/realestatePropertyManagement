using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Role entity data operations.
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the RoleRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RoleRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a role by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role.</param>
    /// <returns>The role entity if found; otherwise null.</returns>
    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// Retrieves all roles from the database.
    /// </summary>
    /// <returns>An enumerable collection of all roles.</returns>
    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    /// <summary>
    /// Creates a new role in the database.
    /// </summary>
    /// <param name="role">The role entity to create.</param>
    /// <returns>The created role entity.</returns>
    public async Task<Role> CreateAsync(Role role)
    {
        _context.Roles.Add(role);
        return role;
    }

    /// <summary>
    /// Updates an existing role in the database.
    /// </summary>
    /// <param name="role">The role entity with updated values.</param>
    public async Task UpdateAsync(Role role)
    {
        _context.Roles.Update(role);
    }

    /// <summary>
    /// Deletes a role from the database by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the role to delete.</param>
    public async Task DeleteAsync(int id)
    {
        var role = await GetByIdAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
        }
    }
}
