using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Payment entity operations.
/// </summary>
public interface IPaymentRepository : IRepository<Payment, Guid>
{
    /// <summary>
    /// Retrieves a page of payments associated with a specific lease via charge-payment relationships.
    /// </summary>
    /// <param name="leaseId">The lease identifier.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of payments for the specified lease.</returns>
    Task<PagedResultDto<Payment>> GetByLeaseIdAsync(Guid leaseId, int pageNumber, int pageSize);

    /// <summary>
    /// Retrieves a payment by its Stripe PaymentIntent identifier.
    /// </summary>
    Task<Payment?> GetByStripePaymentIntentIdAsync(string paymentIntentId);

    /// <summary>
    /// Retrieves every payment across all leases for the admin dashboard, eager loading lease,
    /// property, owner and tenant context. Results are ordered newest first.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound (UTC) on the payment creation date.</param>
    /// <param name="to">Optional inclusive upper bound (UTC) on the payment creation date.</param>
    /// <returns>A collection of payments with full context.</returns>
    Task<IEnumerable<Payment>> GetAllForAdminAsync(DateTime? from, DateTime? to);

    /// <summary>
    /// Retrieves a page of payments across all leases for the admin dashboard, eager loading lease,
    /// property, owner and tenant context. Results are ordered newest first.
    /// </summary>
    /// <param name="from">Optional inclusive lower bound (UTC) on the payment creation date.</param>
    /// <param name="to">Optional inclusive upper bound (UTC) on the payment creation date.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of payments with full context.</returns>
    Task<PagedResultDto<Payment>> GetAllForAdminAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
}
