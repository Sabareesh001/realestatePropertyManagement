using System;
using System.ComponentModel.DataAnnotations;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for creating a bank account.
/// </summary>
public class CreateBankAccountDto
{
    /// <summary>
    /// Gets or sets the name of the bank.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string BankName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the account number.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string AccountNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the account holder.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string AccountHolderName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the IFSC code of the bank branch.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string IfscCode { get; set; } = null!;
}

/// <summary>
/// Data Transfer Object for updating a bank account.
/// </summary>
public class UpdateBankAccountDto
{
    /// <summary>
    /// Gets or sets the name of the bank.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string BankName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the account number.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string AccountNumber { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name of the account holder.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string AccountHolderName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the IFSC code of the bank branch.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string IfscCode { get; set; } = null!;
}

/// <summary>
/// Data Transfer Object for representing a bank account response.
/// </summary>
public class BankAccountResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the bank account.
    /// </summary>
    public Guid Id { get; set; }

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
    /// Gets or sets the date and time when the bank account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
