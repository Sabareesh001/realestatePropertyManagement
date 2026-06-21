using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for CreateChargeDto to ensure charge creation inputs are valid.
/// </summary>
public class CreateChargeDtoValidator : AbstractValidator<CreateChargeDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateChargeDtoValidator"/> class and defines validation rules.
    /// </summary>
    public CreateChargeDtoValidator()
    {
        RuleFor(x => x.ChargeTypeId)
            .GreaterThan(0).WithMessage("Charge type ID is required and must be a positive integer.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required.")
            .Must(date => date >= DateTime.Today)
            .WithMessage("Due date cannot be in the past.");
    }
}
