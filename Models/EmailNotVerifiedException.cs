namespace propertyManagement.Models;

/// <summary>
/// Thrown when a user attempts to log in before verifying their email address.
/// </summary>
public class EmailNotVerifiedException : UnauthorizedAccessException
{
    /// <summary>
    /// Initializes a new instance of the EmailNotVerifiedException class.
    /// </summary>
    public EmailNotVerifiedException()
        : base("Please verify your email before logging in.")
    {
    }
}
