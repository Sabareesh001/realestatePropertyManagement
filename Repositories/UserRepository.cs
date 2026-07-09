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
            .Include(u => u.UserRoles.Where(ur => ur.DeletedAt == null))
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves all users from the database.
    /// </summary>
    /// <returns>An enumerable collection of all users.</returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users
            .Include(u => u.UserRoles.Where(ur => ur.DeletedAt == null))
                .ThenInclude(ur => ur.Role)
            .Where(u => u.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles.Where(ur => ur.DeletedAt == null))
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves a user by their Stripe Connect account identifier.
    /// </summary>
    public async Task<User?> GetByStripeAccountIdAsync(string stripeAccountId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.StripeAccountId == stripeAccountId && u.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves a user by their pending email verification hash.
    /// </summary>
    public async Task<User?> GetByEmailVerificationHashAsync(string hash)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationHash == hash && u.DeletedAt == null);
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
            user.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Users.Update(user);
        }
    }

    public async Task<bool> HasRoleAsync(Guid id,int roleId)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles.Where(ur => ur.DeletedAt == null))
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

        if (user != null)
        {
            var roles = user.UserRoles.Select(ur => ur.RoleId).ToList();
            return roles.Contains(roleId);
        }
        return false;
    }

    /// <summary>
    /// Retrieves the identifiers of all active users assigned to the given role.
    /// </summary>
    public async Task<IEnumerable<Guid>> GetUserIdsByRoleAsync(int roleId)
    {
        return await _context.Users
            .Where(u => u.DeletedAt == null && u.UserRoles.Any(ur => ur.RoleId == roleId && ur.DeletedAt == null))
            .Select(u => u.Id)
            .ToListAsync();
    }
}
