namespace propertyManagement.Models;

public class StripeSettings
{
    public string SecretKey { get; set; } = null!;
    public string PublishableKey { get; set; } = null!;
    public string WebhookSecret { get; set; } = null!;
    public double PlatformFeePercent { get; set; } = 5.0;
    public string Country { get; set; } = "US";
    public string DefaultCurrency { get; set; } = "usd";
    public string OnboardingReturnUrl { get; set; } = null!;
    public string OnboardingRefreshUrl { get; set; } = null!;
}
