using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Stripe;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

public class StripeConnectService : IStripeConnectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StripeSettings _settings;
    private readonly IStripeGateway _stripe;

    private const int StripePaymentMethodId = Models.PaymentMethod.Stripe;

    public StripeConnectService(
        IUnitOfWork unitOfWork,
        IOptions<StripeSettings> settings,
        IStripeGateway stripe)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _stripe = stripe ?? throw new ArgumentNullException(nameof(stripe));
    }

    public async Task<StripeOnboardingResponseDto> OnboardOwnerAsync(Guid ownerId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(ownerId)
            ?? throw new KeyNotFoundException("Owner not found.");

        if (string.IsNullOrEmpty(user.StripeAccountId))
        {
            var account = await _stripe.Accounts.CreateAsync(new AccountCreateOptions
            {
                Type = "express",
                Email = user.Email,
                Country = _settings.Country,
                Capabilities = new AccountCapabilitiesOptions
                {
                    CardPayments = new AccountCapabilitiesCardPaymentsOptions { Requested = true },
                    Transfers = new AccountCapabilitiesTransfersOptions { Requested = true }
                }
            });

            user.StripeAccountId = account.Id;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        var link = await _stripe.AccountLinks.CreateAsync(new AccountLinkCreateOptions
        {
            Account = user.StripeAccountId,
            ReturnUrl = _settings.OnboardingReturnUrl,
            RefreshUrl = _settings.OnboardingRefreshUrl,
            Type = "account_onboarding"
        });

        return new StripeOnboardingResponseDto
        {
            OnboardingUrl = link.Url,
            StripeAccountId = user.StripeAccountId
        };
    }

    public async Task<StripeAccountStatusDto> GetAccountStatusAsync(Guid ownerId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(ownerId)
            ?? throw new KeyNotFoundException("Owner not found.");

        if (string.IsNullOrEmpty(user.StripeAccountId))
        {
            return new StripeAccountStatusDto();
        }

        var account = await _stripe.Accounts.GetAsync(user.StripeAccountId);

        user.StripeChargesEnabled = account.ChargesEnabled;
        user.StripePayoutsEnabled = account.PayoutsEnabled;
        user.StripeDetailsSubmitted = account.DetailsSubmitted;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new StripeAccountStatusDto
        {
            StripeAccountId = user.StripeAccountId,
            ChargesEnabled = account.ChargesEnabled,
            PayoutsEnabled = account.PayoutsEnabled,
            DetailsSubmitted = account.DetailsSubmitted
        };
    }

    public async Task<CreatePaymentIntentResponseDto> CreatePaymentIntentAsync(Guid tenantId, Guid leaseId, RecordPaymentDto dto)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId)
            ?? throw new KeyNotFoundException("Lease not found.");

        if (lease.TenantId != tenantId)
            throw new UnauthorizedAccessException("You are not the tenant for this lease.");

        if (lease.StatusId != Models.LeaseStatus.Active)
            throw new InvalidOperationException("Payments can only be made on active leases.");

        var ownerId = lease.PropertyNavigation?.OwnerId
            ?? throw new InvalidOperationException("Lease has no associated property.");

        var owner = await _unitOfWork.Users.GetByIdAsync(ownerId)
            ?? throw new KeyNotFoundException("Property owner not found.");

        if (!owner.StripeChargesEnabled || !owner.StripePayoutsEnabled)
            throw new InvalidOperationException("The property owner has not completed Stripe onboarding. Payments are not available yet.");

        await _unitOfWork.BeginTransactionAsync();

        var totalAmount = dto.ChargeAllocations.Sum(a => a.Amount);
        var platformFee = Math.Round(totalAmount * (decimal)_settings.PlatformFeePercent / 100m, 2);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            Amount = totalAmount,
            PlatformFeeAmount = platformFee,
            PaymentMethodId = StripePaymentMethodId,
            CurrencyId = dto.CurrencyId,
            PaidBy = tenantId,
            StatusId = PaymentStatus.Pending,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _unitOfWork.Payments.CreateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        foreach (var allocation in dto.ChargeAllocations)
        {
            var charge = await _unitOfWork.Charges.GetByIdWithPaymentsAsync(allocation.ChargeId);
            if (charge == null)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new KeyNotFoundException($"Charge '{allocation.ChargeId}' not found.");
            }

            if (!charge.Leases.Any(l => l.Id == leaseId))
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException($"Charge '{allocation.ChargeId}' does not belong to this lease.");
            }

            if (charge.StatusId == ChargeStatus.Paid || charge.StatusId == ChargeStatus.Cancelled)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException($"Charge '{allocation.ChargeId}' is already {(charge.StatusId == ChargeStatus.Paid ? "paid" : "cancelled")}.");
            }

            var alreadyPaid = charge.ChargePayments
                .Where(cp => cp.Payment.StatusId == PaymentStatus.Completed || cp.Payment.StatusId == PaymentStatus.Pending)
                .Sum(cp => cp.AmountApplied ?? 0);
            var balanceDue = (charge.Amount ?? 0) - alreadyPaid;

            if (allocation.Amount > balanceDue)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new InvalidOperationException($"Payment amount {allocation.Amount} exceeds the balance due {balanceDue} for charge '{allocation.ChargeId}'.");
            }

            charge.ChargePayments.Add(new ChargePayment
            {
                ChargeId = charge.Id,
                PaymentId = payment.Id,
                AmountApplied = allocation.Amount
            });

            await _unitOfWork.Charges.UpdateAsync(charge);
        }

        // Create the Stripe PaymentIntent before committing so we can store the PI id
        var pi = await _stripe.PaymentIntents.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = (long)(totalAmount * 100),
            Currency = _settings.DefaultCurrency,
            TransferGroup = payment.Id.ToString(),
            Metadata = new Dictionary<string, string>
            {
                ["paymentId"] = payment.Id.ToString(),
                ["leaseId"] = leaseId.ToString(),
                ["ownerId"] = ownerId.ToString()
            }
        });

        payment.StripePaymentIntentId = pi.Id;
        await _unitOfWork.Payments.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        return new CreatePaymentIntentResponseDto
        {
            PaymentId = payment.Id,
            ClientSecret = pi.ClientSecret,
            PublishableKey = _settings.PublishableKey,
            Amount = totalAmount,
            PlatformFee = platformFee,
            Currency = _settings.DefaultCurrency
        };
    }

    public async Task HandleWebhookAsync(string json, string stripeSignature)
    {
        var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _settings.WebhookSecret);

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                await HandlePaymentSucceededAsync(stripeEvent);
                break;

            case "payment_intent.payment_failed":
                await HandlePaymentFailedAsync(stripeEvent);
                break;

            case "account.updated":
                await HandleAccountUpdatedAsync(stripeEvent);
                break;
        }
    }

    private async Task HandlePaymentSucceededAsync(Event stripeEvent)
    {
        var pi = stripeEvent.Data.Object as PaymentIntent;
        if (pi == null) return;

        var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(pi.Id);
        if (payment == null || payment.StatusId == PaymentStatus.Completed)
            return;

        if (!pi.Metadata.TryGetValue("ownerId", out var ownerIdStr) || !Guid.TryParse(ownerIdStr, out var ownerId))
            return;

        var owner = await _unitOfWork.Users.GetByIdAsync(ownerId);
        if (owner == null || string.IsNullOrEmpty(owner.StripeAccountId))
            return;

        await _unitOfWork.BeginTransactionAsync();

        payment.StatusId = PaymentStatus.Completed;
        payment.PaidAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        foreach (var cp in payment.ChargePayments)
        {
            var charge = cp.Charge;
            var totalPaidForCharge = charge.ChargePayments
                .Where(x => x.Payment.StatusId == PaymentStatus.Completed || x.PaymentId == payment.Id)
                .Sum(x => x.AmountApplied ?? 0);

            if (totalPaidForCharge >= (charge.Amount ?? 0))
                charge.StatusId = ChargeStatus.Paid;
            else if (totalPaidForCharge > 0)
                charge.StatusId = ChargeStatus.PartiallyPaid;

            await _unitOfWork.Charges.UpdateAsync(charge);
        }

        await _unitOfWork.Payments.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();

        // Transfer owner's share to their connected account AFTER the DB commit so that
        // a commit failure cannot leave an orphaned Stripe transfer (which would double-pay the owner on retry).
        var ownerAmount = (payment.Amount ?? 0) - (payment.PlatformFeeAmount ?? 0);
        if (ownerAmount > 0)
        {
            var transferOptions = new TransferCreateOptions
            {
                Amount = (long)(ownerAmount * 100),
                Currency = _settings.DefaultCurrency,
                Destination = owner.StripeAccountId,
                TransferGroup = payment.Id.ToString()
            };

            // Use the latest charge as source when available to avoid insufficient balance errors
            if (!string.IsNullOrEmpty(pi.LatestChargeId))
                transferOptions.SourceTransaction = pi.LatestChargeId;

            // Idempotency key ensures exactly-once delivery even if the webhook fires twice
            var requestOptions = new RequestOptions { IdempotencyKey = $"transfer-{payment.Id}" };
            var transfer = await _stripe.Transfers.CreateAsync(transferOptions, requestOptions);

            payment.StripeTransferId = transfer.Id;
            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandlePaymentFailedAsync(Event stripeEvent)
    {
        var pi = stripeEvent.Data.Object as PaymentIntent;
        if (pi == null) return;

        var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(pi.Id);
        if (payment == null || payment.StatusId != PaymentStatus.Pending)
            return;

        payment.StatusId = PaymentStatus.Failed;
        await _unitOfWork.Payments.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleAccountUpdatedAsync(Event stripeEvent)
    {
        var account = stripeEvent.Data.Object as Account;
        if (account == null) return;

        var user = await _unitOfWork.Users.GetByStripeAccountIdAsync(account.Id);
        if (user == null)
            return;

        user.StripeChargesEnabled = account.ChargesEnabled;
        user.StripePayoutsEnabled = account.PayoutsEnabled;
        user.StripeDetailsSubmitted = account.DetailsSubmitted;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
