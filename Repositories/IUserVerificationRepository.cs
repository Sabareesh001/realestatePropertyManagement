using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for UserVerification entity operations.
/// </summary>
public interface IUserVerificationRepository : IRepository<UserVerification, Guid>
{
    /// <summary>
    /// Retrieves a page of user verification requests with a status of Pending.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of pending user verification requests.</returns>
    Task<PagedResultDto<UserVerification>> GetPendingVerificationsAsync(int pageNumber, int pageSize);

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
