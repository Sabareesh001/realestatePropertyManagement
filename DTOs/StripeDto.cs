using System;

namespace propertyManagement.DTOs;

public class StripeOnboardingResponseDto
{
    public string OnboardingUrl { get; set; } = null!;
    public string StripeAccountId { get; set; } = null!;
}

public class StripeAccountStatusDto
{
    public string? StripeAccountId { get; set; }
    public bool ChargesEnabled { get; set; }
    public bool PayoutsEnabled { get; set; }
    public bool DetailsSubmitted { get; set; }
    public bool IsOnboarded => ChargesEnabled && PayoutsEnabled && DetailsSubmitted;
}

public class CreatePaymentIntentResponseDto
{
    public Guid PaymentId { get; set; }
    public string ClientSecret { get; set; } = null!;
    public string PublishableKey { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal PlatformFee { get; set; }
    public string Currency { get; set; } = null!;
}
