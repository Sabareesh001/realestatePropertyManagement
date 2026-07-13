namespace propertyManagement.Services;

/// <summary>
/// Service interface for sending transactional emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email verification link to the specified recipient.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="firstName">The recipient's first name, used for personalization.</param>
    /// <param name="verificationLink">The full verification link the recipient should click.</param>
    Task SendVerificationEmailAsync(string toEmail, string firstName, string verificationLink);
}
