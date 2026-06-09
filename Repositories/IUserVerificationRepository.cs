using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for UserVerification entity operations.
/// </summary>
public interface IUserVerificationRepository : IRepository<UserVerification, Guid>
{
    /// <summary>
    /// Retrieves all user verification requests with a status of Pending.
    /// </summary>
    /// <returns>A collection of pending user verification requests.</returns>
    Task<IEnumerable<UserVerification>> GetPendingVerificationsAsync();

    /// <summary>
    /// Retrieves the latest verification request submitted by a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The latest verification entity if found; otherwise null.</returns>
    Task<UserVerification?> GetLatestVerificationByUserIdAsync(Guid userId);

    /// <summary>
    /// Checks if a user is currently verified in the system.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>True if the user is verified; otherwise false.</returns>
    Task<bool> IsUserVerifiedAsync(Guid userId);
}
