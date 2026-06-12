using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace propertyManagement.Controllers;

/// <summary>
/// Base controller class providing shared utilities and helper methods for API controllers.
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Retrieves the unique identifier of the currently authenticated user from the name identifier claim.
    /// </summary>
    /// <returns>The unique identifier of the user as a <see cref="Guid"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the user is not authenticated or the user ID claim is missing or invalid.</exception>
    protected Guid GetCurrentUserId()
    {
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(nameIdentifier) || !Guid.TryParse(nameIdentifier, out var userId))
        {
            throw new InvalidOperationException("User is not authenticated or user ID claim is missing.");
        }
        return userId;
    }
}
