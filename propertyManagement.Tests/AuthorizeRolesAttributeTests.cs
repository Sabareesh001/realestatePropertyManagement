using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using propertyManagement.Filters;

namespace propertyManagement.Tests;

/// <summary>
/// Unit tests for the <see cref="AuthorizeRolesAttribute"/> class.
/// </summary>
[TestFixture]
public class AuthorizeRolesAttributeTests
{
    private ActionContext CreateActionContext(ClaimsPrincipal principal)
    {
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };
        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }

    private AuthorizationFilterContext CreateFilterContext(ActionContext actionContext)
    {
        return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    /// <summary>
    /// Verifies that OnAuthorizationAsync returns 401 Unauthorized if the user is not authenticated.
    /// </summary>
    [Test]
    public async Task OnAuthorizationAsync_UserNotAuthenticated_Returns401Unauthorized()
    {
        // Arrange
        var filter = new AuthorizeRolesAttribute("Owner");
        var identity = new ClaimsIdentity(); // Unauthenticated identity
        var principal = new ClaimsPrincipal(identity);
        var filterContext = CreateFilterContext(CreateActionContext(principal));

        // Act
        await filter.OnAuthorizationAsync(filterContext);

        // Assert
        var result = filterContext.Result as ObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
        
        var problemDetails = result.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status401Unauthorized));
        Assert.That(problemDetails.Title, Is.EqualTo("Unauthorized"));
        Assert.That(problemDetails.Detail, Is.EqualTo("Authentication is required to access this resource."));
    }

    /// <summary>
    /// Verifies that OnAuthorizationAsync returns 403 Forbidden with a custom message if the user lacks the required role.
    /// </summary>
    [Test]
    public async Task OnAuthorizationAsync_UserMissingRequiredRole_Returns403Forbidden_WithCorrectMessage()
    {
        // Arrange
        var filter = new AuthorizeRolesAttribute("Owner");
        var claims = new List<Claim> { new(ClaimTypes.Role, "Tenant") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var filterContext = CreateFilterContext(CreateActionContext(principal));

        // Act
        await filter.OnAuthorizationAsync(filterContext);

        // Assert
        var result = filterContext.Result as ObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status403Forbidden));

        var problemDetails = result.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Status, Is.EqualTo(StatusCodes.Status403Forbidden));
        Assert.That(problemDetails.Title, Is.EqualTo("Forbidden"));
        Assert.That(problemDetails.Detail, Is.EqualTo("Only owner can perform this action"));
    }

    /// <summary>
    /// Verifies that OnAuthorizationAsync allows access if the user has the required role.
    /// </summary>
    [Test]
    public async Task OnAuthorizationAsync_UserHasRequiredRole_AllowsAccess()
    {
        // Arrange
        var filter = new AuthorizeRolesAttribute("Owner");
        var claims = new List<Claim> { new(ClaimTypes.Role, "Owner") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var filterContext = CreateFilterContext(CreateActionContext(principal));

        // Act
        await filter.OnAuthorizationAsync(filterContext);

        // Assert
        Assert.That(filterContext.Result, Is.Null); // Access allowed
    }

    /// <summary>
    /// Verifies that OnAuthorizationAsync formats the error message correctly when multiple roles are allowed but missing.
    /// </summary>
    [Test]
    public async Task OnAuthorizationAsync_MultipleRolesRequired_UserMissingAll_ReturnsCorrectMessage()
    {
        // Arrange
        var filter = new AuthorizeRolesAttribute("Owner", "Admin");
        var claims = new List<Claim> { new(ClaimTypes.Role, "Tenant") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);
        var filterContext = CreateFilterContext(CreateActionContext(principal));

        // Act
        await filter.OnAuthorizationAsync(filterContext);

        // Assert
        var result = filterContext.Result as ObjectResult;
        Assert.That(result, Is.Not.Null);

        var problemDetails = result.Value as ProblemDetails;
        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails.Detail, Is.EqualTo("Only owner or admin can perform this action"));
    }
}
