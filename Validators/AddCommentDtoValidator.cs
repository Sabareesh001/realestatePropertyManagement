using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validates <see cref="AddCommentDto"/> inputs.
/// </summary>
public class AddCommentDtoValidator : AbstractValidator<AddCommentDto>
{
    /// <summary>Initializes validation rules.</summary>
    public AddCommentDtoValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MinimumLength(1).WithMessage("Message must be at least 1 character.")
            .MaximumLength(2000).WithMessage("Message must not exceed 2000 characters.");
    }
}
