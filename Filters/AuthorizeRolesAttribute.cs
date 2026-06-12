using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace propertyManagement.Filters;

/// <summary>
/// Custom authorization filter attribute that enforces role requirements and returns a detailed ProblemDetails response if the check fails.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeRolesAttribute : Attribute, IAsyncAuthorizationFilter
{
    /// <summary>
    /// Gets the list of roles allowed to access the resource.
    /// </summary>
    public string[] Roles { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizeRolesAttribute"/> class.
    /// </summary>
    /// <param name="roles">The list of roles allowed to access the resource.</param>
    public AuthorizeRolesAttribute(params string[] roles)
    {
        Roles = roles ?? Array.Empty<string>();
    }

    /// <summary>
    /// Performs role-based authorization checks asynchronously.
    /// </summary>
    /// <param name="context">The authorization filter context.</param>
    /// <returns>A task representing the asynchronous filter operation.</returns>
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Verify if user is authenticated
        if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Authentication is required to access this resource."
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            return Task.CompletedTask;
        }

        // If no roles are specified, authentication itself is sufficient
        if (Roles.Length == 0)
        {
            return Task.CompletedTask;
        }

        // Check if user is in at least one of the specified roles
        var hasRole = Roles.Any(role => user.IsInRole(role));

        if (!hasRole)
        {
            // Dynamically construct the detail message based on required roles
            string message;
            if (Roles.Length == 1)
            {
                message = $"Only {Roles[0].ToLower()} can perform this action";
            }
            else
            {
                var rolesList = string.Join(" or ", Roles.Select(r => r.ToLower()));
                message = $"Only {rolesList} can perform this action";
            }

            context.Result = new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = message
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Task.CompletedTask;
    }
}
