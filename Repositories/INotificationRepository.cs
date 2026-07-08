using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Notification entity data operations.
/// </summary>
public interface INotificationRepository : IRepository<Notification, Guid>
{
    /// <summary>
    /// Retrieves all notifications addressed to a specific recipient, newest first.
    /// </summary>
    /// <param name="recipientId">The recipient's user identifier.</param>
    /// <returns>A collection of notifications for the recipient.</returns>
    Task<IEnumerable<Notification>> GetByRecipientIdAsync(Guid recipientId);
}
