using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Extensions;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for ResendVerificationDto to ensure the email address is valid.
/// </summary>
public class ResendVerificationDtoValidator : AbstractValidator<ResendVerificationDto>
{
    /// <summary>
    /// Initializes a new instance of the ResendVerificationDtoValidator class and defines validation rules.
    /// </summary>
    public ResendVerificationDtoValidator()
    {
        RuleFor(x => x.Email)
            .ValidEmail();
    }
}
