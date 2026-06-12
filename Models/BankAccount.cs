using System;
using System.Collections.Generic;

namespace propertyManagement.Models;

/// <summary>
/// Represents a bank account entity.
/// </summary>
public class BankAccount
{
    /// <summary>
    /// Gets or sets the unique identifier of the bank account.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the name of the bank.
    /// </summary>
    public string BankName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the account number.
    /// </summary>
    public string AccountNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the account holder.
    /// </summary>
    public string AccountHolderName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the IFSC code of the bank branch.
    /// </summary>
    public string IfscCode { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

    /// <summary>
    /// Gets or sets the date and time when the record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user mappings for this bank account.
    /// </summary>
    public virtual ICollection<UserBankAccount> UserBankAccounts { get; set; } = new List<UserBankAccount>();

    /// <summary>
    /// Gets or sets the date and time when the bank account was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
