using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using propertyManagement.Models;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for sending transactional emails via SMTP using MailKit.
/// </summary>
public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    /// <summary>
    /// Initializes a new instance of the EmailService class.
    /// </summary>
    /// <param name="options">The SMTP configuration options.</param>
    public EmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    /// <summary>
    /// Sends an email verification link to the specified recipient.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="firstName">The recipient's first name, used for personalization.</param>
    /// <param name="verificationLink">The full verification link the recipient should click.</param>
    public async Task SendVerificationEmailAsync(string toEmail, string firstName, string verificationLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Verify your email address";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <p>Hi {firstName},</p>
                <p>Thanks for signing up. Please verify your email address by clicking the link below:</p>
                <p><a href="{verificationLink}">{verificationLink}</a></p>
                <p>This link will expire in 24 hours. If you didn't create an account, you can ignore this email.</p>
                """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
        await client.AuthenticateAsync(_settings.Username, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
