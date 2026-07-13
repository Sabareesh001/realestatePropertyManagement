using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for platform-wide finance queries used by the admin dashboard.
/// </summary>
public interface IAdminFinanceService
{
    /// <summary>
    /// Retrieves a page of payments across all leases, newest first, enriched with lease/property/owner/tenant context.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound on the payment creation date.</param>
    /// <param name="to">Optional inclusive upper bound on the payment creation date.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A paged result of admin payment DTOs.</returns>
    Task<PagedResultDto<AdminPaymentDto>> GetAllPaymentsAsync(DateTime? from, DateTime? to, PaginationParams pagination);

    /// <summary>
    /// Retrieves a page of charges across all leases, newest first, enriched with lease/property/owner/tenant context.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound on the charge creation date.</param>
    /// <param name="to">Optional inclusive upper bound on the charge creation date.</param>
    /// <param name="pagination">The pagination parameters.</param>
    /// <returns>A paged result of admin charge DTOs.</returns>
    Task<PagedResultDto<AdminChargeDto>> GetAllChargesAsync(DateTime? from, DateTime? to, PaginationParams pagination);

    /// <summary>
    /// Computes aggregate finance figures across all payments for the admin dashboard.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound on the payment creation date.</param>
    /// <param name="to">Optional inclusive upper bound on the payment creation date.</param>
    /// <returns>A finance summary DTO.</returns>
    Task<AdminFinanceSummaryDto> GetSummaryAsync(DateTime? from, DateTime? to);
}
