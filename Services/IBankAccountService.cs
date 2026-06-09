using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for bank accounts.
/// </summary>
public interface IBankAccountService
{
    /// <summary>
    /// Creates a new bank account and maps it to the user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="dto">The create bank account details.</param>
    /// <returns>The created bank account details.</returns>
    Task<BankAccountResponseDto> CreateBankAccountAsync(Guid userId, CreateBankAccountDto dto);

    /// <summary>
    /// Retrieves all bank accounts for the user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A collection of bank account responses.</returns>
    Task<IEnumerable<BankAccountResponseDto>> GetUserBankAccountsAsync(Guid userId);

    /// <summary>
    /// Retrieves a specific bank account for the user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bankAccountId">The unique identifier of the bank account.</param>
    /// <returns>The bank account response details.</returns>
    Task<BankAccountResponseDto> GetBankAccountByIdAsync(Guid userId, Guid bankAccountId);

    /// <summary>
    /// Updates a specific bank account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bankAccountId">The unique identifier of the bank account.</param>
    /// <param name="dto">The updated bank account details.</param>
    /// <returns>The updated bank account response details.</returns>
    Task<BankAccountResponseDto> UpdateBankAccountAsync(Guid userId, Guid bankAccountId, UpdateBankAccountDto dto);

    /// <summary>
    /// Deletes a specific bank account and its mappings.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="bankAccountId">The unique identifier of the bank account.</param>
    Task DeleteBankAccountAsync(Guid userId, Guid bankAccountId);
}
