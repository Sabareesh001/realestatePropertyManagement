using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Repositories;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for RegisterDto to ensure user registration inputs are valid.
/// </summary>
public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the RegisterDtoValidator class and defines validation rules.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database verification.</param>
    public RegisterDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        RuleFor(x => x.Email)
            .ValidEmail();

        RuleFor(x => x.Password)
            .ValidPassword();

        RuleFor(x => x.FirstName)
            .ValidPersonName("First name");

        RuleFor(x => x.LastName)
            .ValidPersonName("Last name");

        RuleFor(x => x.Phone)
            .ValidPhone();

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(dob => dob < DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Date of birth cannot be today or a future date.")
            .Must(dob => dob >= new DateOnly(1900, 1, 1))
            .WithMessage("Date of birth must be a realistic date (1900 or later).")
            .Must(dob => dob <= DateOnly.FromDateTime(DateTime.Today.AddYears(-18)))
            .WithMessage("User must be at least 18 years old.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.")
            .MustAsync(async (roleId, cancellationToken) =>
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                return role != null;
            })
            .WithMessage("The specified RoleId does not exist.");
    }
}
