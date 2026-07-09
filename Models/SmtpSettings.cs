namespace propertyManagement.Models;

/// <summary>
/// Configuration settings for sending email via SMTP.
/// </summary>
public class SmtpSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool UseSsl { get; set; } = true;
    public string FromEmail { get; set; } = null!;
    public string FromName { get; set; } = "Property Management";
}
