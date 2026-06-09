using System;

namespace propertyManagement.Models;

/// <summary>
/// Represents the mapping/join table between User and BankAccount.
/// </summary>
public class UserBankAccount
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the user.
    /// </summary>
    public virtual User User { get; set; } = null!;

    /// <summary>
    /// Gets or sets the unique identifier of the bank account.
    /// </summary>
    public Guid BankAccountId { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for the bank account.
    /// </summary>
    public virtual BankAccount BankAccount { get; set; } = null!;
}
