using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for managing bank accounts.
/// </summary>
public class BankAccountService : IBankAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="BankAccountService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public BankAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public async Task<BankAccountResponseDto> CreateBankAccountAsync(Guid userId, CreateBankAccountDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        var bankAccount = new BankAccount
        {
            BankName = dto.BankName,
            AccountNumber = dto.AccountNumber,
            AccountHolderName = dto.AccountHolderName,
            IfscCode = dto.IfscCode,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _unitOfWork.BankAccounts.CreateAsync(bankAccount);
        await _unitOfWork.BankAccounts.AddUserMappingAsync(userId, bankAccount.Id);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(bankAccount);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BankAccountResponseDto>> GetUserBankAccountsAsync(Guid userId)
    {
        var bankAccounts = await _unitOfWork.BankAccounts.GetBankAccountsByUserIdAsync(userId);
        return bankAccounts.Select(MapToResponseDto);
    }

    /// <inheritdoc />
    public async Task<BankAccountResponseDto> GetBankAccountByIdAsync(Guid userId, Guid bankAccountId)
    {
        if (!await _unitOfWork.BankAccounts.IsOwnerAsync(userId, bankAccountId))
        {
            throw new UnauthorizedAccessException("You do not have access to this bank account.");
        }

        var bankAccount = await _unitOfWork.BankAccounts.GetByIdAsync(bankAccountId);
        if (bankAccount == null)
        {
            throw new KeyNotFoundException("Bank account not found.");
        }

        return MapToResponseDto(bankAccount);
    }

    /// <inheritdoc />
    public async Task<BankAccountResponseDto> UpdateBankAccountAsync(Guid userId, Guid bankAccountId, UpdateBankAccountDto dto)
    {
        if (!await _unitOfWork.BankAccounts.IsOwnerAsync(userId, bankAccountId))
        {
            throw new UnauthorizedAccessException("You do not have access to this bank account.");
        }

        var bankAccount = await _unitOfWork.BankAccounts.GetByIdAsync(bankAccountId);
        if (bankAccount == null)
        {
            throw new KeyNotFoundException("Bank account not found.");
        }

        bankAccount.BankName = dto.BankName;
        bankAccount.AccountNumber = dto.AccountNumber;
        bankAccount.AccountHolderName = dto.AccountHolderName;
        bankAccount.IfscCode = dto.IfscCode;
        bankAccount.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _unitOfWork.BankAccounts.UpdateAsync(bankAccount);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(bankAccount);
    }

    /// <inheritdoc />
    public async Task DeleteBankAccountAsync(Guid userId, Guid bankAccountId)
    {
        if (!await _unitOfWork.BankAccounts.IsOwnerAsync(userId, bankAccountId))
        {
            throw new UnauthorizedAccessException("You do not have access to this bank account.");
        }

        await _unitOfWork.BankAccounts.RemoveUserMappingAsync(userId, bankAccountId);
        await _unitOfWork.BankAccounts.DeleteAsync(bankAccountId);
        await _unitOfWork.SaveChangesAsync();
    }

    private static BankAccountResponseDto MapToResponseDto(BankAccount bankAccount)
    {
        return new BankAccountResponseDto
        {
            Id = bankAccount.Id,
            BankName = bankAccount.BankName,
            AccountNumber = bankAccount.AccountNumber,
            AccountHolderName = bankAccount.AccountHolderName,
            IfscCode = bankAccount.IfscCode,
            CreatedAt = bankAccount.CreatedAt
        };
    }
}
