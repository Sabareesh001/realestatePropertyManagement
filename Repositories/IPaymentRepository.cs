using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for Payment entity operations.
/// </summary>
public interface IPaymentRepository : IRepository<Payment, Guid>
{
    /// <summary>
    /// Retrieves all payments associated with a specific lease via charge-payment relationships.
    /// </summary>
    Task<IEnumerable<Payment>> GetByLeaseIdAsync(Guid leaseId);

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
}
