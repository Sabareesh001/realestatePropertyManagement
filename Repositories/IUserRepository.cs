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
