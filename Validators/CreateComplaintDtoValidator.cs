using FluentValidation;
using propertyManagement.DTOs;

namespace propertyManagement.Validators;

/// <summary>
/// Validates <see cref="CreateComplaintDto"/> inputs.
/// Business-rule checks (active lease, caller is tenant) live in the service layer.
/// </summary>
public class CreateComplaintDtoValidator : AbstractValidator<CreateComplaintDto>
{
    /// <summary>Initializes validation rules.</summary>
    public CreateComplaintDtoValidator()
    {
        RuleFor(x => x.LeaseId)
            .NotEmpty().WithMessage("Lease ID is required.");

        RuleFor(x => x.CategoryId)
            .InclusiveBetween(1, 8).WithMessage("Category must be between 1 and 8.");

        RuleFor(x => x.PriorityId)
            .InclusiveBetween(1, 4).WithMessage("Priority must be between 1 and 4.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MinimumLength(5).WithMessage("Subject must be at least 5 characters.")
            .MaximumLength(150).WithMessage("Subject must not exceed 150 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(x => x.AttachmentUrl)
            .Must(url => string.IsNullOrEmpty(url) ||
                (Uri.TryCreate(url, UriKind.Absolute, out var parsed) &&
                 (parsed.Scheme == Uri.UriSchemeHttp || parsed.Scheme == Uri.UriSchemeHttps)))
            .WithMessage("Attachment URL must be a valid absolute URL.");
    }
}
