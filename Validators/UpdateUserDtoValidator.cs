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
            .MinimumLength(2).WithMessage("First name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s'\-]+$").WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes.")
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty.")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s'\-]+$").WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes.")
            .When(x => x.LastName != null);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number cannot be empty.")
            .Matches(@"^\+?[\d\s\-\(\)]{7,15}$").WithMessage("Phone number must be 7–15 digits and may include +, spaces, hyphens, or parentheses.")
            .Must(phone =>
            {
                var digitsOnly = System.Text.RegularExpressions.Regex.Replace(phone ?? "", @"\D", "");
                return digitsOnly.Length >= 7 && digitsOnly.Length <= 15;
            }).WithMessage("Phone number must contain between 7 and 15 digits.")
            .When(x => x.Phone != null);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth cannot be empty.")
            .Must(dob => dob < DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of birth cannot be today or a future date.")
            .Must(dob => dob >= new DateOnly(1900, 1, 1))
            .WithMessage("Date of birth must be a realistic date (1900 or later).")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.Today.AddYears(-18)))
            .WithMessage("User must be at least 18 years old.")
            .When(x => x.DateOfBirth != null);
    }
}
