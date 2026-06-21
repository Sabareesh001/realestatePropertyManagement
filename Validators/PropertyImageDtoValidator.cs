using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for PropertyImageDto to ensure correct data when updating/syncing property images.
/// </summary>
public class PropertyImageDtoValidator : AbstractValidator<PropertyImageDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyImageDtoValidator"/> class and defines validation rules.
    /// </summary>
    public PropertyImageDtoValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
    }
}
