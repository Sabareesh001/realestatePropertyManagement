using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for Payment entity operations.
/// </summary>
public class PaymentRepository : IPaymentRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PaymentRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a payment by its identifier, eager loading related entities.
    /// </summary>
    /// <param name="id">The payment identifier.</param>
    /// <returns>The payment entity if found; otherwise null.</returns>
    public async Task<Payment?> GetByIdAsync(Guid id)
    {
        return await _context.Payments
            .Include(p => p.PaymentMethod)
            .Include(p => p.Status)
            .Include(p => p.Currency)
            .Include(p => p.ChargePayments)
                .ThenInclude(cp => cp.Charge)
            .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
    }

    /// <summary>
    /// Retrieves all payments, eager loading related entities.
    /// </summary>
    /// <returns>A collection of all payments.</returns>
    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments
            .Include(p => p.PaymentMethod)
            .Include(p => p.Status)
            .Where(p => p.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all payments associated with a specific lease via charge-payment relationships.
    /// </summary>
    /// <param name="leaseId">The lease identifier.</param>
    /// <returns>A collection of payments linked to the specified lease.</returns>
    public async Task<IEnumerable<Payment>> GetByLeaseIdAsync(Guid leaseId)
    {
        return await _context.Payments
            .Include(p => p.PaymentMethod)
            .Include(p => p.Status)
            .Include(p => p.Currency)
            .Include(p => p.ChargePayments)
                .ThenInclude(cp => cp.Charge)
            .Where(p => p.DeletedAt == null &&
                p.ChargePayments.Any(cp =>
                    cp.Charge.Leases.Any(l => l.Id == leaseId)))
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new payment record.
    /// </summary>
    /// <param name="entity">The payment to create.</param>
    /// <returns>The created payment.</returns>
    public async Task<Payment> CreateAsync(Payment entity)
    {
        entity.CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        await _context.Payments.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Updates an existing payment record.
    /// </summary>
    /// <param name="entity">The payment to update.</param>
    public async Task UpdateAsync(Payment entity)
    {
        entity.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        _context.Payments.Update(entity);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Soft deletes a payment record by its identifier.
    /// </summary>
    /// <param name="id">The payment identifier.</param>
    public async Task DeleteAsync(Guid id)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);
        if (payment != null)
        {
            payment.DeletedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            _context.Payments.Update(payment);
        }
    }
}
