using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using propertyManagement.Data;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository implementation for bank accounts.
/// </summary>
public class BankAccountRepository : IBankAccountRepository
{
    private readonly PropertyManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BankAccountRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public BankAccountRepository(PropertyManagementDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<BankAccount> CreateAsync(BankAccount entity)
    {
        var result = await _context.BankAccounts.AddAsync(entity);
        return result.Entity;
    }

    /// <inheritdoc />
    public async Task<BankAccount?> GetByIdAsync(Guid id)
    {
        return await _context.BankAccounts.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BankAccount>> GetAllAsync()
    {
        return await _context.BankAccounts.ToListAsync();
    }

    /// <inheritdoc />
    public Task UpdateAsync(BankAccount entity)
    {
        _context.BankAccounts.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id)
    {
        var bankAccount = await GetByIdAsync(id);
        if (bankAccount != null)
        {
            _context.BankAccounts.Remove(bankAccount);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BankAccount>> GetBankAccountsByUserIdAsync(Guid userId)
    {
        return await _context.UserBankAccounts
            .Where(uba => uba.UserId == userId)
            .Select(uba => uba.BankAccount)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task AddUserMappingAsync(Guid userId, Guid bankAccountId)
    {
        var mapping = new UserBankAccount
        {
            UserId = userId,
            BankAccountId = bankAccountId
        };
        await _context.UserBankAccounts.AddAsync(mapping);
    }

    /// <inheritdoc />
    public async Task RemoveUserMappingAsync(Guid userId, Guid bankAccountId)
    {
        var mapping = await _context.UserBankAccounts
            .FirstOrDefaultAsync(uba => uba.UserId == userId && uba.BankAccountId == bankAccountId);
        if (mapping != null)
        {
            _context.UserBankAccounts.Remove(mapping);
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsOwnerAsync(Guid userId, Guid bankAccountId)
    {
        return await _context.UserBankAccounts
            .AnyAsync(uba => uba.UserId == userId && uba.BankAccountId == bankAccountId);
    }
}
