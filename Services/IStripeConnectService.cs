using System;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

public interface IStripeConnectService
{
    Task<StripeOnboardingResponseDto> OnboardOwnerAsync(Guid ownerId);
    Task<StripeAccountStatusDto> GetAccountStatusAsync(Guid ownerId);
    Task<CreatePaymentIntentResponseDto> CreatePaymentIntentAsync(Guid tenantId, Guid leaseId, RecordPaymentDto dto);
    Task HandleWebhookAsync(string json, string stripeSignature);
}
