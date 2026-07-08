using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace propertyManagement.Hubs;

/// <summary>
/// SignalR hub used to push real-time lease/lease-proposal approval notifications to connected clients.
/// Server-push only; clients do not invoke methods on this hub.
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    /// <summary>
    /// Joins the connecting user to their personal notification group.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        if (userId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, PersonalGroupName(userId.Value));
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Gets the SignalR group name for a user's personal notification channel.
    /// </summary>
    public static string PersonalGroupName(Guid userId) => $"user:{userId}";

    private Guid? GetUserId()
    {
        var nameIdentifier = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(nameIdentifier, out var userId) ? userId : null;
    }
}
