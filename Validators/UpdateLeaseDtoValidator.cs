using FluentValidation;
using propertyManagement.DTOs;
using propertyManagement.Models;
using System;

namespace propertyManagement.Validators;

/// <summary>
/// Validator for UpdateLeaseDto to ensure lease update inputs are valid.
/// </summary>
public class UpdateLeaseDtoValidator : AbstractValidator<UpdateLeaseDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateLeaseDtoValidator"/> class and defines validation rules.
    /// </summary>
    public UpdateLeaseDtoValidator()
    {
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
            .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Start date cannot be in the past.")
            .When(x => x.StartDate.HasValue);

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate ?? DateOnly.MinValue)
            .WithMessage("End date must be after the start date.")
            .Must((dto, endDate) => endDate >= (dto.StartDate ?? DateOnly.MinValue).AddMonths(1))
            .WithMessage("Lease duration must be at least 1 month.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => x.AgreementDocumentUrl)
            .NotEmpty().WithMessage("Agreement document URL is required when submitting a lease.")
            .When(x => x.StatusId.HasValue && x.StatusId.Value == LeaseStatus.Submitted);
    }
}
