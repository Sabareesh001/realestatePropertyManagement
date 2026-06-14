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

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate ?? DateOnly.MinValue).WithMessage("End date must be after the start date.")
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);

        RuleFor(x => x.AgreementDocumentUrl)
            .NotEmpty().WithMessage("Agreement document URL is required when submitting a lease.")
            .When(x => x.StatusId.HasValue && x.StatusId.Value == LeaseStatus.Submitted);
    }
}
