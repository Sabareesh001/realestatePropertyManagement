using FluentValidation;
using propertyManagement.DTOs;
using System;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for CreateLeaseProposalDto to ensure lease proposal inputs are valid.
/// </summary>
public class CreateLeaseProposalDtoValidator : AbstractValidator<CreateLeaseProposalDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLeaseProposalDtoValidator"/> class and defines validation rules.
    /// </summary>
    public CreateLeaseProposalDtoValidator()
    {
        RuleFor(x => x.PropertyId)
            .GreaterThan(0).WithMessage("Property ID must be greater than zero.");

        RuleFor(x => x.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly rent cannot be negative.")
            .When(x => x.MonthlyRent.HasValue);

        RuleFor(x => x.UpfrontPayment)
            .GreaterThanOrEqualTo(0).WithMessage("Upfront payment cannot be negative.")
            .When(x => x.UpfrontPayment.HasValue);

        RuleFor(x => x.SecurityDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Security deposit cannot be negative.")
            .When(x => x.SecurityDeposit.HasValue);

        RuleFor(x => x.StartDate)
            .Must((dto, startDate) =>
            {
                if (!startDate.HasValue) return true;
                return startDate.Value >= DateOnly.FromDateTime(DateTime.Today);
            })
            .WithMessage("Start date cannot be in the past.")
            .When(x => x.StartDate.HasValue);

        RuleFor(x => x.EndDate)
            .Must((dto, endDate) =>
            {
                if (!endDate.HasValue || !dto.StartDate.HasValue) return true;
                return endDate.Value > dto.StartDate.Value;
            })
            .WithMessage("End date must be after start date.")
            .Must((dto, endDate) =>
            {
                if (!endDate.HasValue || !dto.StartDate.HasValue) return true;
                return endDate.Value >= dto.StartDate.Value.AddMonths(1);
            })
            .WithMessage("Lease proposal duration must be at least 1 month.")
            .When(x => x.EndDate.HasValue && x.StartDate.HasValue);
    }
}
