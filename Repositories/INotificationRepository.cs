using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Notification entity data operations.
/// </summary>
public interface INotificationRepository : IRepository<Notification, Guid>
{
    /// <summary>
    /// Retrieves a page of notifications addressed to a specific recipient, newest first.
    /// </summary>
    /// <param name="recipientId">The recipient's user identifier.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of notifications for the recipient.</returns>
    Task<PagedResultDto<Notification>> GetByRecipientIdAsync(Guid recipientId, int pageNumber, int pageSize);
}
