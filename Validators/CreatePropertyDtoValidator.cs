using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Repositories;
using System;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for CreatePropertyDto to ensure property creation inputs are valid.
/// </summary>
public class CreatePropertyDtoValidator : AbstractValidator<CreatePropertyDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePropertyDtoValidator"/> class and defines validation rules.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database verification.</param>
    /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
    public CreatePropertyDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        RuleFor(x => x.Title)
            .ValidPropertyTitle();

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line is required.")
            .MinimumLength(5).WithMessage("Address line must be at least 5 characters.")
            .MaximumLength(300).WithMessage("Address line cannot exceed 300 characters.");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("City ID must be greater than zero.")
            .MustAsync(async (cityId, cancellationToken) =>
            {
                var city = await _unitOfWork.Cities.GetByIdAsync(cityId);
                return city != null;
            })
            .WithMessage("The specified city does not exist.");

        RuleFor(x => x.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly rent cannot be negative.");

        RuleFor(x => x.UpfrontPayment)
            .GreaterThanOrEqualTo(0).WithMessage("Upfront payment cannot be negative.");

        RuleFor(x => x)
            .Must(x => x.MonthlyRent > 0 || x.UpfrontPayment > 0)
            .WithMessage("Monthly rent and upfront payment cannot both be zero simultaneously.")
            .WithName("MonthlyRent");

        RuleFor(x => x.SecurityDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Security deposit cannot be negative.");

        RuleForEach(x => x.PropertyImages)
            .SetValidator(new CreatePropertyImageDtoValidator());
    }
}
