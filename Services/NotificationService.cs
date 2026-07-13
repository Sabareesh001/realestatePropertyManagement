using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Hubs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for persisting and pushing real-time notifications over SignalR.
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationHub> _hubContext;

    /// <summary>
    /// Initializes a new instance of <see cref="NotificationService"/>.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database access.</param>
    /// <param name="hubContext">The SignalR hub context used to push real-time notifications.</param>
    public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    /// <inheritdoc/>
    public async Task NotifyAsync(
        IReadOnlyCollection<Guid> recipientIds,
        int notificationTypeId,
        string title,
        string message,
        string? relatedEntityType = null,
        Guid? relatedEntityId = null)
    {
        if (recipientIds == null || recipientIds.Count == 0)
        {
            return;
        }

        var notifications = recipientIds.Select(recipientId => new Notification
        {
            Id = Guid.NewGuid(),
            RecipientId = recipientId,
            TypeId = notificationTypeId,
            Title = title,
            Message = message,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId
        }).ToList();

        foreach (var notification in notifications)
        {
            await _unitOfWork.Notifications.CreateAsync(notification);
        }

        await _unitOfWork.SaveChangesAsync();

        // Each recipient's own connections were joined to their personal group on connect
        // (see NotificationHub.OnConnectedAsync); pushing there works regardless of reconnects
        // or multiple open tabs, and lets every recipient receive a DTO carrying their own row id.
        foreach (var notification in notifications)
        {
            var groupName = NotificationHub.PersonalGroupName(notification.RecipientId);
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", MapToResponseDto(notification));
        }
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<NotificationResponseDto>> GetMyNotificationsAsync(Guid userId, PaginationParams pagination)
    {
        var notifications = await _unitOfWork.Notifications.GetByRecipientIdAsync(userId, pagination.PageNumber, pagination.PageSize);
        return notifications.Select(MapToResponseDto);
    }

    /// <inheritdoc/>
    public async Task MarkAsReadAsync(Guid userId, Guid notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification == null)
        {
            throw new KeyNotFoundException("Notification not found.");
        }

        if (notification.RecipientId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this notification.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            await _unitOfWork.Notifications.UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private static NotificationResponseDto MapToResponseDto(Notification notification)
    {
        return new NotificationResponseDto
        {
            Id = notification.Id,
            TypeId = notification.TypeId,
            Title = notification.Title,
            Message = notification.Message,
            RelatedEntityType = notification.RelatedEntityType,
            RelatedEntityId = notification.RelatedEntityId,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}
