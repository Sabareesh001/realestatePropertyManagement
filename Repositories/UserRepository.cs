using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for User entity data operations.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UserRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>The user entity if found; otherwise null.</returns>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>An enumerable collection of all users.</returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The user entity if found; otherwise null.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <returns>The created user entity with its ID populated.</returns>
    public async Task<User> CreateAsync(User user)
    {
        user.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Users.Add(user);
        return user;
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="user">The user entity with updated values.</param>
    public async Task UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Users.Update(user);
    }

    /// <summary>
    /// Deletes a user from the database by their identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    public async Task DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }
    }
}
