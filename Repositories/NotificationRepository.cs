using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Notification entity data operations.
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="NotificationRepository"/>.
    /// </summary>
    /// <param name="context">The database context.</param>
    public NotificationRepository(PropertyManagementDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.DeletedAt == null);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Notification>> GetAllAsync()
    {
        return await _context.Notifications
            .Where(n => n.DeletedAt == null)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<Notification>> GetByRecipientIdAsync(Guid recipientId, int pageNumber, int pageSize)
    {
        return await _context.Notifications
            .Where(n => n.RecipientId == recipientId && n.DeletedAt == null)
            .OrderByDescending(n => n.CreatedAt)
            .ToPagedResultAsync(pageNumber, pageSize);
    }

    /// <inheritdoc/>
    public async Task<Notification> CreateAsync(Notification entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Notifications.Add(entity);
        return entity;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Notification entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Notifications.Update(entity);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        var notification = await GetByIdAsync(id);
        if (notification != null)
        {
            notification.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Notifications.Update(notification);
        }
    }
}
