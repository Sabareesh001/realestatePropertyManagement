using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for SubmitVerificationDto to ensure user verification submission inputs are valid.
/// </summary>
public class SubmitVerificationDtoValidator : AbstractValidator<SubmitVerificationDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubmitVerificationDtoValidator"/> class and defines validation rules.
    /// </summary>
    public SubmitVerificationDtoValidator()
    {
        RuleFor(x => x.Documents)
            .NotEmpty().WithMessage("At least one verification document is required.");

        RuleForEach(x => x.Documents)
            .SetValidator(new VerificationDocumentDtoValidator());
    }
}
