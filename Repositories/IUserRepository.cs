using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for User entity data operations.
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    /// <summary>
    /// Retrieves a page of all users.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of users.</returns>
    Task<PagedResultDto<User>> GetAllAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Retrieves a user by their Stripe Connect account identifier.
    /// </summary>
    Task<User?> GetByStripeAccountIdAsync(string stripeAccountId);

    /// <summary>
    /// Retrieves a user by their pending email verification hash.
    /// </summary>
    Task<User?> GetByEmailVerificationHashAsync(string hash);

    Task<bool> HasRoleAsync(Guid userId, int roleId);

    /// <summary>
    /// Retrieves the identifiers of all active users assigned to the given role.
    /// </summary>
    /// <param name="roleId">The role identifier (see <see cref="propertyManagement.Models.Role"/>).</param>
    /// <returns>A collection of user identifiers holding the specified role.</returns>
    Task<IEnumerable<Guid>> GetUserIdsByRoleAsync(int roleId);
}
