using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for ComplaintComment entity data operations.
/// </summary>
public interface IComplaintCommentRepository : IRepository<ComplaintComment, Guid>
{
    /// <summary>
    /// Retrieves all comments for a complaint, ordered by creation time, with author navigation loaded.
    /// </summary>
    /// <param name="complaintId">The complaint identifier.</param>
    /// <returns>A collection of comments.</returns>
    Task<IEnumerable<ComplaintComment>> GetByComplaintIdAsync(Guid complaintId);
}
