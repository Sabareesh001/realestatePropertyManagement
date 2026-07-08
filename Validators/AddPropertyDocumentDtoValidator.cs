using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for <see cref="AddPropertyDocumentDto"/> to ensure property document inputs are valid.
/// </summary>
public class AddPropertyDocumentDtoValidator : AbstractValidator<AddPropertyDocumentDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddPropertyDocumentDtoValidator"/> class and defines validation rules.
    /// </summary>
    public AddPropertyDocumentDtoValidator()
    {
        RuleFor(x => x.DocumentTypeId)
            .InclusiveBetween(1, 5)
            .WithMessage("Document type must be valid (1–5).");

        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("Document number is required.")
            .MinimumLength(4).WithMessage("Document number must be at least 4 characters.")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("Document number can only contain letters, digits, and hyphens.");

        RuleFor(x => x.DocumentUrl)
            .NotEmpty().WithMessage("Document URL is required.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Document URL must be a valid absolute URL.");
    }
}
