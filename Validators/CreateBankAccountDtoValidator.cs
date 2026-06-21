using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Extensions;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for CreateBankAccountDto to ensure bank account creation inputs are valid.
/// </summary>
public class CreateBankAccountDtoValidator : AbstractValidator<CreateBankAccountDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateBankAccountDtoValidator"/> class and defines validation rules.
    /// </summary>
    public CreateBankAccountDtoValidator()
    {
        RuleFor(x => x.BankName)
            .NotEmpty().WithMessage("Bank name is required.")
            .MinimumLength(2).WithMessage("Bank name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Bank name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s\-\.&]+$").WithMessage("Bank name can only contain letters, spaces, hyphens, periods, and ampersands.");

        RuleFor(x => x.AccountNumber)
            .ValidBankAccountNumber();

        RuleFor(x => x.AccountHolderName)
            .ValidPersonName("Account holder name");

        RuleFor(x => x.IfscCode)
            .ValidIfscCode();
    }
}
