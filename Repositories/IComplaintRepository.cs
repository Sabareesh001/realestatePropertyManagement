using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;
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
    /// Retrieves a page of complaints with related entities eager-loaded.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of complaints.</returns>
    Task<PagedResultDto<Complaint>> GetAllWithDetailsAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a page of complaints created by the specified user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of complaints.</returns>
    Task<PagedResultDto<Complaint>> GetByCreatedByAsync(Guid userId, int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a page of complaints for properties owned by the specified user.
    /// </summary>
    /// <param name="ownerId">The owner's user identifier.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of complaints.</returns>
    Task<PagedResultDto<Complaint>> GetByOwnerIdAsync(Guid ownerId, int pageNumber, int pageSize);
}
