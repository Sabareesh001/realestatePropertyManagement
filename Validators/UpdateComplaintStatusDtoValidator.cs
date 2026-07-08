using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validates <see cref="UpdateComplaintStatusDto"/> inputs.
/// </summary>
public class UpdateComplaintStatusDtoValidator : AbstractValidator<UpdateComplaintStatusDto>
{
    /// <summary>Initializes validation rules.</summary>
    public UpdateComplaintStatusDtoValidator()
    {
        RuleFor(x => x.StatusId)
            .InclusiveBetween(1, 5).WithMessage("Status must be between 1 and 5.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Note must not exceed 500 characters.")
            .When(x => x.Note != null);
    }
}
