using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for <see cref="AddLeaseDocumentDto"/> to ensure tenant-supplied lease document inputs are valid.
/// </summary>
public class AddLeaseDocumentDtoValidator : AbstractValidator<AddLeaseDocumentDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddLeaseDocumentDtoValidator"/> class and defines validation rules.
    /// </summary>
    public AddLeaseDocumentDtoValidator()
    {
        RuleFor(x => x.DocumentTypeId)
            .InclusiveBetween(1, 5)
            .WithMessage("Document type must be valid (1–5).");

        RuleFor(x => x.DocumentNumber)
            .MinimumLength(4).WithMessage("Document number must be at least 4 characters.")
            .MaximumLength(50).WithMessage("Document number cannot exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("Document number can only contain letters, digits, and hyphens.")
            .When(x => !string.IsNullOrEmpty(x.DocumentNumber));

        RuleFor(x => x.DocumentUrl)
            .NotEmpty().WithMessage("Document URL is required.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Document URL must be a valid absolute URL.");
    }
}
