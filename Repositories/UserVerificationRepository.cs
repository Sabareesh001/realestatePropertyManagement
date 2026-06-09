using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for UserVerification entity data operations.
/// </summary>
public class UserVerificationRepository : IUserVerificationRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserVerificationRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public UserVerificationRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a user verification by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user verification.</param>
    /// <returns>The UserVerification entity if found; otherwise null.</returns>
    public async Task<UserVerification?> GetByIdAsync(Guid id)
    {
        return await _context.UserVerifications
            .Include(uv => uv.UserVerificationDocuments)
                .ThenInclude(uvd => uvd.Document)
            .FirstOrDefaultAsync(uv => uv.Id == id);
    }

    /// <summary>
    /// Retrieves all user verifications from the database.
    /// </summary>
    /// <returns>An enumerable collection of all UserVerifications.</returns>
    public async Task<IEnumerable<UserVerification>> GetAllAsync()
    {
        return await _context.UserVerifications
            .Include(uv => uv.UserVerificationDocuments)
                .ThenInclude(uvd => uvd.Document)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new user verification request.
    /// </summary>
    /// <param name="entity">The user verification request to create.</param>
    /// <returns>The created user verification entity.</returns>
    public async Task<UserVerification> CreateAsync(UserVerification entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.UserVerifications.Add(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing user verification request.
    /// </summary>
    /// <param name="entity">The user verification request to update.</param>
    public async Task UpdateAsync(UserVerification entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.UserVerifications.Update(entity);
    }

    /// <summary>
    /// Deletes a user verification request.
    /// </summary>
    /// <param name="id">The identifier of the request to delete.</param>
    public async Task DeleteAsync(Guid id)
    {
        var verification = await GetByIdAsync(id);
        if (verification != null)
        {
            _context.UserVerifications.Remove(verification);
        }
    }

    /// <summary>
    /// Retrieves all user verification requests with a status of Pending.
    /// </summary>
    /// <returns>A collection of pending user verification requests.</returns>
    public async Task<IEnumerable<UserVerification>> GetPendingVerificationsAsync()
    {
        return await _context.UserVerifications
            .Include(uv => uv.UserVerificationDocuments)
                .ThenInclude(uvd => uvd.Document)
            .Where(uv => uv.Status == "Pending")
            .OrderBy(uv => uv.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves the latest verification request submitted by a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The latest verification entity if found; otherwise null.</returns>
    public async Task<UserVerification?> GetLatestVerificationByUserIdAsync(Guid userId)
    {
        return await _context.UserVerifications
            .Include(uv => uv.UserVerificationDocuments)
                .ThenInclude(uvd => uvd.Document)
            .Where(uv => uv.UserId == userId)
            .OrderByDescending(uv => uv.CreatedAt)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Checks if a user is currently verified in the system.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>True if the user is verified; otherwise false.</returns>
    public async Task<bool> IsUserVerifiedAsync(Guid userId)
    {
        return await _context.UserVerifications
            .AnyAsync(uv => uv.UserId == userId && uv.Status == "Verified");
    }
}
