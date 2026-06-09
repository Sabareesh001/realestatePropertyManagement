using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for managing BankAccount and UserBankAccount mappings.
/// </summary>
public interface IBankAccountRepository : IRepository<BankAccount, Guid>
{
    /// <summary>
    /// Gets all bank accounts associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of bank accounts.</returns>
    Task<IEnumerable<BankAccount>> GetBankAccountsByUserIdAsync(Guid userId);

    /// <summary>
    /// Adds a mapping between a user and a bank account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bankAccountId">The unique identifier of the bank account.</param>
    Task AddUserMappingAsync(Guid userId, Guid bankAccountId);

    /// <summary>
    /// Removes the mapping between a user and a bank account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bankAccountId">The unique identifier of the bank account.</param>
    Task RemoveUserMappingAsync(Guid userId, Guid bankAccountId);

    /// <summary>
    /// Determines whether the user is mapped to the bank account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bankAccountId">The unique identifier of the bank account.</param>
    /// <returns>True if mapped; otherwise, false.</returns>
    Task<bool> IsOwnerAsync(Guid userId, Guid bankAccountId);
}
