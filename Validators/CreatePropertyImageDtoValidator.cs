using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for CreatePropertyImageDto to ensure correct data when adding property images.
/// </summary>
public class CreatePropertyImageDtoValidator : AbstractValidator<CreatePropertyImageDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePropertyImageDtoValidator"/> class and defines validation rules.
    /// </summary>
    public CreatePropertyImageDtoValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
    }
}
