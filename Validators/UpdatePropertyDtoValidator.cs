using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Repositories;
using System;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for UpdatePropertyDto to ensure property update inputs are valid.
/// </summary>
public class UpdatePropertyDtoValidator : AbstractValidator<UpdatePropertyDto>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePropertyDtoValidator"/> class and defines validation rules.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database verification.</param>
    /// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
    public UpdatePropertyDtoValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");

        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address line is required.");

        RuleFor(x => x.CityId)
            .GreaterThan(0).WithMessage("City ID must be valid")
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
            .SetValidator(new PropertyImageDtoValidator());
    }
}
