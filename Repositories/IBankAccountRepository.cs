using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;

namespace propertyManagement.Repositories;

/// <summary>
/// Repository interface for managing BankAccount and UserBankAccount mappings.
/// </summary>
public interface IBankAccountRepository : IRepository<BankAccount, Guid>
{
    /// <summary>
    /// Gets a page of bank accounts associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paged result of bank accounts.</returns>
    Task<PagedResultDto<BankAccount>> GetBankAccountsByUserIdAsync(Guid userId, int pageNumber, int pageSize);

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
