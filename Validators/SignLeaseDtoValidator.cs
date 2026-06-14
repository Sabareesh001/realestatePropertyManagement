using FluentValidation;
using propertyManagement.DTOs;
using System;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for SignLeaseDto to ensure signature inputs are valid.
/// </summary>
public class SignLeaseDtoValidator : AbstractValidator<SignLeaseDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SignLeaseDtoValidator"/> class and defines validation rules.
    /// </summary>
    public SignLeaseDtoValidator()
    {
        RuleFor(x => x.SignedAgreementDocumentUrl)
            .NotEmpty().WithMessage("Signed agreement document URL is required.");
    }
}
