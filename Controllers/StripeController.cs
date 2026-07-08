using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

[ApiController]
[Route("api/stripe")]
public class StripeController : BaseApiController
{
    private readonly IStripeConnectService _stripeService;

    public StripeController(IStripeConnectService stripeService)
    {
        _stripeService = stripeService ?? throw new ArgumentNullException(nameof(stripeService));
    }

    /// <summary>
    /// Creates a Stripe Express Connect account for the owner and returns an onboarding URL.
    /// </summary>
    [Authorize(Roles = "Owner")]
    [HttpPost("connect/onboard")]
    public async Task<ActionResult<StripeOnboardingResponseDto>> OnboardOwner()
    {
        var ownerId = GetCurrentUserId();
        var result = await _stripeService.OnboardOwnerAsync(ownerId);
        return Ok(result);
    }

    /// <summary>
    /// Returns the Stripe Connect account status for the authenticated owner.
    /// </summary>
    [Authorize(Roles = "Owner")]
    [HttpGet("connect/status")]
    public async Task<ActionResult<StripeAccountStatusDto>> GetAccountStatus()
    {
        var ownerId = GetCurrentUserId();
        var result = await _stripeService.GetAccountStatusAsync(ownerId);
        return Ok(result);
    }

    /// <summary>
    /// Creates a Stripe PaymentIntent for tenant to pay lease charges.
    /// Returns a client_secret the frontend uses to confirm payment with Stripe.js.
    /// </summary>
    [Authorize(Roles = "Tenant")]
    [HttpPost("lease/{leaseId:guid}/payments/intent")]
    public async Task<ActionResult<CreatePaymentIntentResponseDto>> CreatePaymentIntent(
        Guid leaseId,
        [FromBody] RecordPaymentDto dto)
    {
        var tenantId = GetCurrentUserId();
        var result = await _stripeService.CreatePaymentIntentAsync(tenantId, leaseId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Stripe webhook endpoint. Handles payment_intent.succeeded, payment_intent.payment_failed,
    /// and account.updated events.
    /// </summary>
    [AllowAnonymous]
    [DisableRequestSizeLimit]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook()
    {
        string json;
        using (var reader = new StreamReader(HttpContext.Request.Body))
        {
            json = await reader.ReadToEndAsync();
        }

        var signature = Request.Headers["Stripe-Signature"].ToString();

        try
        {
            await _stripeService.HandleWebhookAsync(json, signature);
            return Ok();
        }
        catch (StripeException)
        {
            // Signature verification failures and other Stripe errors must return 400
            // so Stripe stops retrying. A 5xx would cause indefinite retry loops.
            return BadRequest();
        }
    }
}
