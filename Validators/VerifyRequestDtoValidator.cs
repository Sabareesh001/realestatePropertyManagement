using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for VerifyRequestDto to ensure verification approval or rejection inputs are valid.
/// </summary>
public class VerifyRequestDtoValidator : AbstractValidator<VerifyRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyRequestDtoValidator"/> class and defines validation rules.
    /// </summary>
    public VerifyRequestDtoValidator()
    {
        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks cannot exceed 500 characters.");
    }
}
