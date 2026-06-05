using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for UpdateUserDto to ensure user update inputs are valid.
/// </summary>
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserDtoValidator class and defines validation rules.
    /// </summary>
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty.")
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty.")
            .When(x => x.LastName != null);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number cannot be empty.")
            .When(x => x.Phone != null);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth cannot be empty.")
            .When(x => x.DateOfBirth != null);
    }
}
