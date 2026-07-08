using Stripe;

namespace propertyManagement.Services;

public class StripeGateway : IStripeGateway
{
    public AccountService Accounts { get; }
    public AccountLinkService AccountLinks { get; }
    public PaymentIntentService PaymentIntents { get; }
    public TransferService Transfers { get; }

    public StripeGateway(IStripeClient stripeClient)
    {
        Accounts = new AccountService(stripeClient);
        AccountLinks = new AccountLinkService(stripeClient);
        PaymentIntents = new PaymentIntentService(stripeClient);
        Transfers = new TransferService(stripeClient);
    }
}
