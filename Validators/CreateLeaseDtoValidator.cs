using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Models;
using System;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for CreateLeaseDto to ensure lease creation inputs are valid.
/// </summary>
public class CreateLeaseDtoValidator : AbstractValidator<CreateLeaseDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateLeaseDtoValidator"/> class and defines validation rules.
    /// </summary>
    public CreateLeaseDtoValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Tenant ID is required.");

        RuleFor(x => x.PropertyId)
            .GreaterThan(0).WithMessage("Property ID must be greater than zero.");

        RuleFor(x => x.ProposalId)
            .NotEmpty().WithMessage("Proposal ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.")
            .Must((dto, endDate) => endDate >= dto.StartDate.AddMonths(1))
            .WithMessage("Lease duration must be at least 1 month.");

        RuleFor(x => x.MonthlyRent)
            .GreaterThanOrEqualTo(0).WithMessage("Monthly rent cannot be negative.");

        RuleFor(x => x.UpfrontPayment)
            .GreaterThanOrEqualTo(0).WithMessage("Upfront payment cannot be negative.");

        RuleFor(x => x.SecurityDeposit)
            .GreaterThanOrEqualTo(0).WithMessage("Security deposit cannot be negative.");
    }
}
