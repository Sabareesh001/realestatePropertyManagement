using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validates <see cref="PaginationParams"/> inputs.
/// </summary>
public class PaginationParamsValidator : AbstractValidator<PaginationParams>
{
    /// <summary>Initializes validation rules.</summary>
    public PaginationParamsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
