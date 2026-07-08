using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using propertyManagement.DTOs;
using propertyManagement.Services;

namespace propertyManagement.Controllers;

/// <summary>
/// API controller exposing platform-wide, admin-only finance data for the admin dashboard.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminFinanceController : BaseApiController
{
    private readonly IAdminFinanceService _adminFinanceService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminFinanceController"/> class.
    /// </summary>
    /// <param name="adminFinanceService">The admin finance service.</param>
    public AdminFinanceController(IAdminFinanceService adminFinanceService)
    {
        _adminFinanceService = adminFinanceService ?? throw new ArgumentNullException(nameof(adminFinanceService));
    }

    /// <summary>
    /// Retrieves every payment across all leases, newest first, with lease/property/owner/tenant context.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound on the payment creation date.</param>
    /// <param name="to">Optional inclusive upper bound on the payment creation date.</param>
    /// <returns>A list of platform-wide payments.</returns>
    /// <response code="200">Payments retrieved successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not an administrator.</response>
    [HttpGet("payments")]
    public async Task<ActionResult<IEnumerable<AdminPaymentDto>>> GetPayments([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _adminFinanceService.GetAllPaymentsAsync(from, to);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves every charge across all leases, newest first, with lease/property/owner/tenant context.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound on the charge creation date.</param>
    /// <param name="to">Optional inclusive upper bound on the charge creation date.</param>
    /// <returns>A list of platform-wide charges.</returns>
    /// <response code="200">Charges retrieved successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not an administrator.</response>
    [HttpGet("charges")]
    public async Task<ActionResult<IEnumerable<AdminChargeDto>>> GetCharges([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _adminFinanceService.GetAllChargesAsync(from, to);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves server-side aggregated finance figures across all payments.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound on the payment creation date.</param>
    /// <param name="to">Optional inclusive upper bound on the payment creation date.</param>
    /// <returns>A finance summary.</returns>
    /// <response code="200">Summary computed successfully.</response>
    /// <response code="401">If the user is unauthorized.</response>
    /// <response code="403">If the user is not an administrator.</response>
    [HttpGet("finance-summary")]
    public async Task<ActionResult<AdminFinanceSummaryDto>> GetFinanceSummary([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _adminFinanceService.GetSummaryAsync(from, to);
        return Ok(result);
    }
}
