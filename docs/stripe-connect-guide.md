# Stripe Connect — Team Onboarding Guide

---

## 1. What Is Stripe?

Stripe is a payment infrastructure company. At its core it gives your backend a REST API to:
- Accept card/wallet payments from users
- Hold and move money between accounts
- Pay out funds to bank accounts

You never touch raw card numbers. The user's browser sends card data **directly to Stripe's servers** via Stripe.js; Stripe gives you a token/secret your backend uses to charge the card. PCI compliance is almost entirely Stripe's problem, not yours.

**Reference:** [Stripe Docs Home](https://stripe.com/docs)

---

## 2. What Is Stripe Connect — and Why Do We Need It?

Standard Stripe is one business receiving money from customers. **Connect** is for *platforms* — a business that facilitates payments between two other parties.

Our situation:

```
Tenant (payer) ──pays──▶ Platform (us) ──transfers──▶ Owner (recipient)
```

We are the **platform**. We take a percentage fee. The owner receives the rest in their own bank account. Neither the platform nor the owner ever sees raw card data — Stripe handles all of it.

Stripe Connect has three account types for recipients (owners):

| Type | Who hosts onboarding UI | Who owns compliance | Effort |
|------|------------------------|---------------------|--------|
| **Express** ✅ (we use this) | Stripe | Shared | Low |
| Standard | Stripe | The connected account | Medium |
| Custom | Your platform | You | Very high |

**Express** means: Stripe gives the owner a pre-built onboarding form and a Stripe Express dashboard. We (the platform) control when payouts happen and what the fee is. It is the right choice for a landlord/tenant marketplace.

**Reference:** [Stripe Connect Overview](https://stripe.com/docs/connect)

---

## 3. Money Flow in Our Implementation

### Step-by-step

```
1. Owner completes Stripe onboarding
   → owner gets a Stripe Express connected account (acct_xxxx)

2. Tenant requests a PaymentIntent for their lease charges
   → we create a PaymentIntent on our platform account
   → Stripe returns a client_secret

3. Frontend gives client_secret to Stripe.js
   → Stripe.js collects card details directly from the tenant
   → Stripe.js confirms the payment (card is charged)
   → money lands in OUR platform Stripe balance

4. Stripe fires a webhook event: payment_intent.succeeded
   → our server receives it
   → we mark the Payment record Completed in our DB
   → we create a Transfer: platform balance → owner's connected account
   → we record the Transfer ID

5. Stripe's payout scheduler (daily/weekly)
   → moves money from owner's Stripe balance → owner's real bank account
   → we don't touch this; Stripe does it automatically
```

### Where the platform fee comes in

When we create the Transfer in step 4:

```
Tenant paid:  ₹10,000
Platform fee: ₹500  (5%)  ← stays in OUR Stripe balance
Transfer:     ₹9,500      → owner's connected account
```

The fee stays in the platform account automatically — we simply transfer *less* than the full amount. The fee percentage is configured in `appsettings.json` under `Stripe:PlatformFeePercent`.

**Reference:** [Separate Charges and Transfers](https://stripe.com/docs/connect/separate-charges-and-transfers)

---

## 4. Core Stripe Concepts Explained

### 4.1 PaymentIntent

A **PaymentIntent** (`pi_xxxx`) represents *one attempt to collect money*. Think of it as a "payment session":
- You create it server-side with the amount and currency
- It has a `status` that changes: `requires_payment_method` → `requires_confirmation` → `succeeded` / `payment_failed`
- The `client_secret` is a one-time token the frontend passes to Stripe.js to complete the payment

We create one PaymentIntent per tenant payment. We store its ID (`StripePaymentIntentId`) on the `Payment` record in our DB so the webhook can look it up later.

**Reference:** [PaymentIntents API](https://stripe.com/docs/api/payment_intents)

### 4.2 Transfer

A **Transfer** (`tr_xxxx`) moves money from **your platform's Stripe balance** to a **connected account's Stripe balance**. It is NOT a bank transfer — that is a Payout, which Stripe initiates automatically on its schedule.

In our code (`StripeConnectService.cs`, `HandlePaymentSucceededAsync`):

```csharp
var transfer = await transferService.CreateAsync(new TransferCreateOptions
{
    Amount        = (long)(ownerAmount * 100), // Stripe uses smallest unit (paise/cents)
    Currency      = "usd",
    Destination   = owner.StripeAccountId,     // acct_xxxx of the owner
    TransferGroup = payment.Id.ToString()      // links PI + Transfer for reconciliation
});
```

`TransferGroup` is a free-form string that groups related transactions in the Stripe dashboard. We use the internal `Payment.Id` so you can search for it.

`SourceTransaction` tells Stripe: "use the funds from this specific charge as the source". Without it, Stripe draws from your rolling balance — which could be zero in test mode.

**Reference:** [Transfers API](https://stripe.com/docs/api/transfers)

### 4.3 Connected Account (`acct_xxxx`)

Every owner who completes onboarding gets a **connected account** — their own Stripe sub-account linked to our platform. The owner:
- Sees their payouts in the Stripe Express dashboard
- Has their own bank account registered with Stripe
- Cannot charge the platform; the platform controls transfers to them

Three key boolean flags Stripe sets on the account (we cache these on the `User` model):

| Flag | Meaning |
|------|---------|
| `ChargesEnabled` | Can the account receive funds? |
| `PayoutsEnabled` | Can Stripe pay out to their bank? |
| `DetailsSubmitted` | Did they finish the onboarding form? |

We require `ChargesEnabled && PayoutsEnabled` before allowing a tenant to pay.

**Reference:** [Account Object](https://stripe.com/docs/api/accounts/object)

### 4.4 Webhooks

Webhooks are **HTTP POST requests Stripe sends to your server** when something happens (payment succeeded, account updated, refund created, etc.). They are the only reliable way to know if a payment actually succeeded — you cannot trust the frontend to tell you, because the user could close their browser or tamper with a response.

Our webhook endpoint: `POST /api/stripe/webhook`

Flow:
1. Stripe sends a POST with a JSON body and a `Stripe-Signature` header
2. We call `EventUtility.ConstructEvent(json, signature, webhookSecret)` — this **verifies** the request came from Stripe (prevents forgery)
3. We switch on `event.Type` and handle accordingly

**Idempotency is critical.** Stripe may send the same event more than once (it retries if your server returns a non-200 response). Our code guards against this:

```csharp
if (payment == null || payment.StatusId == PaymentStatus.Completed)
    return; // already processed — skip silently
```

**Reference:** [Webhooks Guide](https://stripe.com/docs/webhooks)

### 4.5 AccountLink (Onboarding URL)

To onboard an owner, we:
1. Create a Stripe Express Account for them (`AccountService.CreateAsync`)
2. Create an **AccountLink** — a short-lived URL (expires in minutes) that Stripe generates
3. Redirect the owner to that URL — Stripe's UI collects their business details, bank account, and ID verification
4. When they finish, Stripe redirects them back to our `OnboardingReturnUrl` (configured in `appsettings.json`)
5. Stripe fires `account.updated` — we sync the `ChargesEnabled/PayoutsEnabled/DetailsSubmitted` flags to our DB

**Reference:** [Express Onboarding](https://stripe.com/docs/connect/express-accounts)

---

## 5. How Our Code Is Structured

```
StripeController.cs
├── POST /api/stripe/connect/onboard              → OnboardOwnerAsync()
├── GET  /api/stripe/connect/status               → GetAccountStatusAsync()
├── POST /api/stripe/lease/{id}/payments/intent   → CreatePaymentIntentAsync()
└── POST /api/stripe/webhook                      → HandleWebhookAsync()

StripeConnectService.cs
├── OnboardOwnerAsync()
│   ├── AccountService.CreateAsync()         ← creates Express account on Stripe
│   └── AccountLinkService.CreateAsync()     ← generates onboarding URL
│
├── GetAccountStatusAsync()
│   └── AccountService.GetAsync()            ← fetches + syncs ChargesEnabled etc.
│
├── CreatePaymentIntentAsync()
│   ├── Validates lease, tenant, charges
│   ├── Creates Payment (StatusId=Pending) + ChargePayment rows in our DB
│   └── PaymentIntentService.CreateAsync()   ← creates the PI on Stripe
│
└── HandleWebhookAsync()
    ├── EventUtility.ConstructEvent()        ← validates Stripe-Signature header
    ├── "payment_intent.succeeded"
    │   ├── Idempotency check
    │   ├── Marks Payment → Completed
    │   ├── Updates Charge statuses (Paid / PartiallyPaid)
    │   └── TransferService.CreateAsync()    ← sends owner's share
    ├── "payment_intent.payment_failed"
    │   └── Marks Payment → Failed
    └── "account.updated"
        └── Syncs StripeChargesEnabled / PayoutsEnabled on User
```

### Model additions

| Model | New field | Purpose |
|-------|-----------|---------|
| `User` | `StripeAccountId` | Owner's `acct_xxxx` |
| `User` | `StripeChargesEnabled` | Cached from Stripe — guards payment creation |
| `User` | `StripePayoutsEnabled` | Cached from Stripe — guards payment creation |
| `User` | `StripeDetailsSubmitted` | Cached from Stripe |
| `Payment` | `StripePaymentIntentId` | `pi_xxxx` — lets webhook find our DB record |
| `Payment` | `StripeTransferId` | `tr_xxxx` — audit trail |
| `Payment` | `PlatformFeeAmount` | Computed at PI creation, used at transfer time |

### Configuration (`appsettings.json`)

```json
"Stripe": {
  "SecretKey": "",           // ← set via user-secrets, never commit
  "PublishableKey": "",      // ← sent to frontend so Stripe.js can initialize
  "WebhookSecret": "",       // ← set via user-secrets, never commit
  "PlatformFeePercent": 5.0, // ← adjust as needed
  "Country": "US",
  "DefaultCurrency": "usd",
  "OnboardingReturnUrl": "http://localhost:4200/owner/stripe/return",
  "OnboardingRefreshUrl": "http://localhost:4200/owner/stripe/refresh"
}
```

---

## 6. Test Mode vs Live Mode

Stripe has a complete **test environment** that behaves identically to production. Nothing costs real money. You switch between them with different API keys:

- `sk_test_...` / `pk_test_...` → test mode
- `sk_live_...` / `pk_live_...` → live mode

### Test card numbers

| Card number | Result |
|-------------|--------|
| `4242 4242 4242 4242` | Payment succeeds |
| `4000 0000 0000 0002` | Card declined |
| `4000 0025 0000 3155` | Requires 3D Secure authentication |

Use any future expiry (e.g. `12/34`) and any 3-digit CVC.

**Reference:** [Testing](https://stripe.com/docs/testing)

---

## 7. Local Development Setup

### Step 1 — Get API keys

1. Create a free account at [dashboard.stripe.com](https://dashboard.stripe.com)
2. Go to **Developers → API keys** (make sure the "Test mode" toggle is ON)
3. Copy the **Secret key** (`sk_test_...`) and **Publishable key** (`pk_test_...`)

### Step 2 — Store keys in user-secrets (never commit to git)

```bash
dotnet user-secrets set "Stripe:SecretKey" "sk_test_YOUR_KEY"
dotnet user-secrets set "Stripe:PublishableKey" "pk_test_YOUR_KEY"
```

### Step 3 — Install the Stripe CLI

```bash
# macOS
brew install stripe/stripe-cli/stripe

stripe login   # opens browser, authenticate with your Stripe account
```

**Reference:** [Stripe CLI install](https://stripe.com/docs/stripe-cli#install)

### Step 4 — Forward webhooks to your local server

```bash
stripe listen --forward-to https://localhost:7xxx/api/stripe/webhook
# Prints: > Ready! Your webhook signing secret is whsec_...
```

Copy the `whsec_...` value and store it:

```bash
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_YOUR_SECRET"
```

> **Why is this needed?** Your local machine isn't reachable from the public internet, so Stripe can't POST webhooks to it directly. The Stripe CLI acts as a tunnel: Stripe sends events to Stripe's servers → CLI receives them → CLI forwards to your localhost.

### Step 5 — Run the migration and start the app

```bash
dotnet ef database update
dotnet run
```

### Step 6 — Test the full flow

```bash
# 1. Log in as an Owner and get the onboarding URL
POST /api/stripe/connect/onboard
# → copy the returned onboardingUrl, open it in a browser
# → fill in Stripe's form using test data (any fake business details)

# 2. Check that onboarding completed
GET /api/stripe/connect/status
# → chargesEnabled: true, payoutsEnabled: true

# 3. Apply a charge to an active lease (as Owner)
POST /api/lease/{leaseId}/charges

# 4. As Tenant, create a PaymentIntent
POST /api/stripe/lease/{leaseId}/payments/intent
# → returns { clientSecret, publishableKey, amount, platformFee }

# 5. Confirm the payment using the Stripe CLI (simulates Stripe.js)
stripe payment_intents confirm pi_xxx --payment-method pm_card_visa

# 6. Watch the Stripe CLI terminal — you will see:
# payment_intent.succeeded  → our server handles it → Transfer created
```

---

## 8. Common Gotchas

| Issue | Why | Fix |
|-------|-----|-----|
| Transfer fails with "insufficient funds" | Platform has no balance in test mode | We pass `SourceTransaction = pi.LatestChargeId` — already handled |
| Webhook returns 400 "No signatures found" | Wrong `WebhookSecret` | Use the `whsec_` printed by `stripe listen`, not from the dashboard |
| Owner can't receive payment — 400 error | `ChargesEnabled` or `PayoutsEnabled` is false | Owner must complete Stripe onboarding fully |
| Same event handled twice | Stripe retries on non-200 responses | Our code checks `payment.StatusId == Completed` and skips — idempotency guard |
| Platform fee is 0 | `PlatformFeePercent` missing | Set it in `appsettings.json` or user-secrets |
| Live mode issues in India | Stripe India Connect has legal restrictions | Stay in test mode for development; review Stripe India docs before going live |

---

## 9. Stripe Object ID Prefixes — Quick Reference

Every Stripe object has a prefix so you can identify it at a glance:

| Prefix | Object |
|--------|--------|
| `pi_` | PaymentIntent |
| `acct_` | Connected Account (owner) |
| `tr_` | Transfer |
| `ch_` | Charge |
| `pm_` | PaymentMethod |
| `evt_` | Webhook Event |
| `whsec_` | Webhook signing secret |
| `sk_test_` / `sk_live_` | Secret API key (backend only) |
| `pk_test_` / `pk_live_` | Publishable key (safe for frontend) |

---

## 10. Further Reading

| Topic | Link |
|-------|------|
| Connect overview | https://stripe.com/docs/connect |
| Express accounts | https://stripe.com/docs/connect/express-accounts |
| Separate charges & transfers | https://stripe.com/docs/connect/separate-charges-and-transfers |
| PaymentIntents guide | https://stripe.com/docs/payments/payment-intents |
| Webhooks guide | https://stripe.com/docs/webhooks |
| Testing | https://stripe.com/docs/testing |
| Stripe CLI | https://stripe.com/docs/stripe-cli |
| Stripe API reference | https://stripe.com/docs/api |
| Stripe.net SDK (GitHub) | https://github.com/stripe/stripe-dotnet |
