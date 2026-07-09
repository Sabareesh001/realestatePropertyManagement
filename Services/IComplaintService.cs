using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for managing complaints and their comment threads.
/// </summary>
public interface IComplaintService
{
    /// <summary>
    /// Creates a new complaint on behalf of the calling tenant.
    /// </summary>
    /// <param name="tenantId">The authenticated tenant's user identifier.</param>
    /// <param name="dto">The complaint creation payload.</param>
    /// <returns>The created complaint.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the lease is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when caller is not the lease's tenant.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the lease is not Active.</exception>
    Task<ComplaintResponseDto> CreateComplaintAsync(Guid tenantId, CreateComplaintDto dto);

    /// <summary>
    /// Retrieves a page of complaints created by the calling user, with an empty comment list.
    /// </summary>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A paged result of complaints.</returns>
    Task<PagedResultDto<ComplaintResponseDto>> GetMyComplaintsAsync(Guid userId, PaginationParams pagination);

    /// <summary>
    /// Retrieves a page of complaints received on properties owned by the calling owner, with empty comment lists.
    /// </summary>
    /// <param name="ownerId">The authenticated owner's user identifier.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A paged result of complaints.</returns>
    Task<PagedResultDto<ComplaintResponseDto>> GetReceivedComplaintsAsync(Guid ownerId, PaginationParams pagination);

    /// <summary>
    /// Retrieves a page of all complaints in the system (admin view), with empty comment lists.
    /// </summary>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A paged result of complaints.</returns>
    Task<PagedResultDto<ComplaintResponseDto>> GetAllComplaintsAsync(PaginationParams pagination);

    /// <summary>
    /// Retrieves a single complaint with full comment thread. Only accessible to the tenant, owner, or admin.
    /// </summary>
    /// <param name="id">The complaint identifier.</param>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="roles">The authenticated user's roles.</param>
    /// <returns>The complaint with full comments.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the complaint is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when caller is not a participant or admin.</exception>
    Task<ComplaintResponseDto> GetComplaintByIdAsync(Guid id, Guid userId, IEnumerable<string> roles);

    /// <summary>
    /// Applies a status transition to a complaint, optionally inserting a note as a comment.
    /// </summary>
    /// <param name="id">The complaint identifier.</param>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="roles">The authenticated user's roles.</param>
    /// <param name="dto">The status update payload.</param>
    /// <returns>The updated complaint with refreshed comments.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the complaint is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when caller lacks permission for this transition.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the transition is not valid from the current status.</exception>
    Task<ComplaintResponseDto> UpdateStatusAsync(Guid id, Guid userId, IEnumerable<string> roles, UpdateComplaintStatusDto dto);

    /// <summary>
    /// Adds a comment to a complaint thread. Only accessible to participants and admins.
    /// </summary>
    /// <param name="id">The complaint identifier.</param>
    /// <param name="userId">The authenticated user identifier.</param>
    /// <param name="roles">The authenticated user's roles.</param>
    /// <param name="dto">The comment payload.</param>
    /// <returns>The created comment DTO.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the complaint is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when caller is not a participant or admin.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the complaint status does not allow comments.</exception>
    Task<ComplaintCommentDto> AddCommentAsync(Guid id, Guid userId, IEnumerable<string> roles, AddCommentDto dto);
}
