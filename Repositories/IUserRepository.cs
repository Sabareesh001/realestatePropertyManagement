using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for User entity data operations.
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>The user entity if found; otherwise null.</returns>
    Task<User?> GetByEmailAsync(string email);
}
