using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Complaint entity data operations.
/// </summary>
public interface IComplaintRepository : IRepository<Complaint, Guid>
{
    /// <summary>
    /// Retrieves a complaint by its identifier with all related entities eager-loaded.
    /// </summary>
    /// <param name="id">The complaint identifier.</param>
    /// <returns>The complaint with navigations if found; otherwise null.</returns>
    new Task<Complaint?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all complaints with related entities eager-loaded.
    /// </summary>
    /// <returns>A collection of complaints.</returns>
    Task<IEnumerable<Complaint>> GetAllWithDetailsAsync();

    /// <summary>
    /// Retrieves all complaints created by the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A collection of complaints.</returns>
    Task<IEnumerable<Complaint>> GetByCreatedByAsync(Guid userId);

    /// <summary>
    /// Retrieves all complaints for properties owned by the specified user.
    /// </summary>
    /// <param name="ownerId">The owner's user identifier.</param>
    /// <returns>A collection of complaints.</returns>
    Task<IEnumerable<Complaint>> GetByOwnerIdAsync(Guid ownerId);
}
