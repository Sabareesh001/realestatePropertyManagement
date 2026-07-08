using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using propertyManagement.Hubs;
using propertyManagement.Models;
using propertyManagement.Repositories;
using propertyManagement.Services;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="NotificationService"/> class.
/// </summary>
[TestFixture]
public class NotificationServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<INotificationRepository> _mockNotificationRepository;
    private Mock<IHubContext<NotificationHub>> _mockHubContext;
    private Mock<IHubClients> _mockClients;
    private Mock<IClientProxy> _mockClientProxy;
    private NotificationService _notificationService;

    [SetUp]
    public void SetUp()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockNotificationRepository = new Mock<INotificationRepository>();
        _mockHubContext = new Mock<IHubContext<NotificationHub>>();
        _mockClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();

        _mockUnitOfWork.Setup(u => u.Notifications).Returns(_mockNotificationRepository.Object);
        _mockNotificationRepository.Setup(r => r.CreateAsync(It.IsAny<Notification>()))
            .ReturnsAsync((Notification n) => n);

        _mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
        _mockHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);

        _notificationService = new NotificationService(_mockUnitOfWork.Object, _mockHubContext.Object);
    }

    [Test]
    public async Task NotifyAsync_SingleRecipient_PersistsAndPushesToPersonalGroup()
    {
        var recipientId = Guid.NewGuid();

        await _notificationService.NotifyAsync(
            new[] { recipientId },
            NotificationType.ProposalSubmitted,
            "Title",
            "Message");

        _mockNotificationRepository.Verify(r => r.CreateAsync(It.Is<Notification>(n => n.RecipientId == recipientId)), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mockClients.Verify(c => c.Group(NotificationHub.PersonalGroupName(recipientId)), Times.Once);
        _mockClientProxy.Verify(p => p.SendCoreAsync("ReceiveNotification", It.IsAny<object[]>(), default), Times.Once);
    }

    [Test]
    public async Task NotifyAsync_MultipleRecipients_PersistsOneRowPerRecipientAndPushesToEachPersonalGroup()
    {
        var ownerId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        await _notificationService.NotifyAsync(
            new[] { ownerId, tenantId },
            NotificationType.LeaseTemplateApproved,
            "Title",
            "Message",
            relatedEntityType: "Lease",
            relatedEntityId: Guid.NewGuid());

        _mockNotificationRepository.Verify(r => r.CreateAsync(It.IsAny<Notification>()), Times.Exactly(2));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mockClients.Verify(c => c.Group(NotificationHub.PersonalGroupName(ownerId)), Times.Once);
        _mockClients.Verify(c => c.Group(NotificationHub.PersonalGroupName(tenantId)), Times.Once);
        _mockClientProxy.Verify(p => p.SendCoreAsync("ReceiveNotification", It.IsAny<object[]>(), default), Times.Exactly(2));
    }

    [Test]
    public async Task NotifyAsync_NoRecipients_DoesNothing()
    {
        await _notificationService.NotifyAsync(
            Array.Empty<Guid>(),
            NotificationType.ProposalSubmitted,
            "Title",
            "Message");

        _mockNotificationRepository.Verify(r => r.CreateAsync(It.IsAny<Notification>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task MarkAsReadAsync_WrongRecipient_ThrowsUnauthorizedAccessException()
    {
        var notificationId = Guid.NewGuid();
        var actualRecipientId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        _mockNotificationRepository.Setup(r => r.GetByIdAsync(notificationId))
            .ReturnsAsync(new Notification { Id = notificationId, RecipientId = actualRecipientId });

        Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _notificationService.MarkAsReadAsync(otherUserId, notificationId));
    }

    [Test]
    public async Task MarkAsReadAsync_CorrectRecipient_MarksAsRead()
    {
        var notificationId = Guid.NewGuid();
        var recipientId = Guid.NewGuid();
        var notification = new Notification { Id = notificationId, RecipientId = recipientId, IsRead = false };

        _mockNotificationRepository.Setup(r => r.GetByIdAsync(notificationId)).ReturnsAsync(notification);

        await _notificationService.MarkAsReadAsync(recipientId, notificationId);

        Assert.That(notification.IsRead, Is.True);
        _mockNotificationRepository.Verify(r => r.UpdateAsync(notification), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
