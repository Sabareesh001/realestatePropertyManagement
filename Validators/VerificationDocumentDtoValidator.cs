using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for VerificationDocumentDto to ensure user verification document inputs are valid.
/// </summary>
public class VerificationDocumentDtoValidator : AbstractValidator<VerificationDocumentDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerificationDocumentDtoValidator"/> class and defines validation rules.
    /// </summary>
    public VerificationDocumentDtoValidator()
    {
        RuleFor(x => x.DocumentTypeId)
            .InclusiveBetween(1, 4)
            .WithMessage("Document type must be valid");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("Document number is required.")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters.");

        RuleFor(x => x.DocumentUrl)
            .NotEmpty().WithMessage("Document URL is required.");
    }
}
