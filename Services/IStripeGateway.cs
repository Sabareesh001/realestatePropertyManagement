using Stripe;

namespace propertyManagement.Services;

/// <summary>
/// Groups all Stripe SDK service classes into a single injectable dependency,
/// mirroring the role IUnitOfWork plays for database repositories.
/// </summary>
public interface IStripeGateway
{
    AccountService Accounts { get; }
    AccountLinkService AccountLinks { get; }
    PaymentIntentService PaymentIntents { get; }
    TransferService Transfers { get; }
}
