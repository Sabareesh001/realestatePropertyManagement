using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Extensions;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for LoginDto to ensure user login inputs are valid.
/// </summary>
public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    /// <summary>
    /// Initializes a new instance of the LoginDtoValidator class and defines validation rules.
    /// </summary>
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .ValidEmail();

        // Login only needs NotEmpty — complexity checks would reject valid stored passwords
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
