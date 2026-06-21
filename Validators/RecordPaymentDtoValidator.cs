using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for RecordPaymentDto to ensure payment recording inputs are valid.
/// </summary>
public class RecordPaymentDtoValidator : AbstractValidator<RecordPaymentDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecordPaymentDtoValidator"/> class and defines validation rules.
    /// </summary>
    public RecordPaymentDtoValidator()
    {
        RuleFor(x => x.ChargeAllocations)
            .NotEmpty().WithMessage("At least one charge allocation is required.");

        RuleForEach(x => x.ChargeAllocations).ChildRules(allocation =>
        {
            allocation.RuleFor(a => a.ChargeId)
                .NotEmpty().WithMessage("Charge ID is required for each allocation.");

            allocation.RuleFor(a => a.Amount)
                .GreaterThan(0).WithMessage("Allocation amount must be greater than zero.");
        });

        RuleFor(x => x.PaymentMethodId)
            .GreaterThan(0).WithMessage("Payment method ID is required and must be a positive integer.");

        RuleFor(x => x.TransactionRef)
            .NotEmpty().WithMessage("Transaction reference is required.")
            .MinimumLength(4).WithMessage("Transaction reference must be at least 4 characters.")
            .MaximumLength(100).WithMessage("Transaction reference cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z0-9\-_]+$").WithMessage("Transaction reference can only contain letters, digits, hyphens, and underscores.");

        RuleFor(x => x.CurrencyId)
            .GreaterThan(0).WithMessage("Currency ID must be a positive integer.");
    }
}
