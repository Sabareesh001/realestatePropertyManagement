using FluentValidation;
using propertyManagement.DTOs;

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
            .MaximumLength(100).WithMessage("Bank name cannot exceed 100 characters.");

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .MaximumLength(50).WithMessage("Account number cannot exceed 50 characters.");

        RuleFor(x => x.AccountHolderName)
            .NotEmpty().WithMessage("Account holder name is required.")
            .MaximumLength(100).WithMessage("Account holder name cannot exceed 100 characters.");

        RuleFor(x => x.IfscCode)
            .NotEmpty().WithMessage("IFSC code is required.")
            .MaximumLength(20).WithMessage("IFSC code cannot exceed 20 characters.");
    }
}
