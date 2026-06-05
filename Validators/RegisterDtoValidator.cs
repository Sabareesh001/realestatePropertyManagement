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
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.");

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
