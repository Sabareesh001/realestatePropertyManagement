using FluentValidation;
using propertyManagement.DTOs;
using System;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for <see cref="UpdateLeaseProposalDto"/> to ensure updated lease proposal inputs are valid.
/// </summary>
public class UpdateLeaseProposalDtoValidator : AbstractValidator<UpdateLeaseProposalDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLeaseProposalDtoValidator"/> class and defines validation rules.
    /// </summary>
    public UpdateLeaseProposalDtoValidator()
    {
        RuleFor(x => x.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly rent cannot be negative.");

        RuleFor(x => x.UpfrontPayment)
            .GreaterThanOrEqualTo(0).WithMessage("Upfront payment cannot be negative.");

        RuleFor(x => x.SecurityDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Security deposit cannot be negative.");

        RuleFor(x => x.StartDate)
            .NotEqual(default(DateOnly)).WithMessage("Start date is required.")
            .Must(startDate => startDate >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.EndDate)
            .NotEqual(default(DateOnly)).WithMessage("End date is required.")
            .Must((dto, endDate) => endDate > dto.StartDate)
            .WithMessage("End date must be after start date.")
            .Must((dto, endDate) => endDate >= dto.StartDate.AddMonths(1))
            .WithMessage("Lease proposal duration must be at least 1 month.");
    }
}
