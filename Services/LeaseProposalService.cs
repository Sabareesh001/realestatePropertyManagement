using System;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for lease proposal-related business operations.
/// </summary>
public class LeaseProposalService : ILeaseProposalService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseProposalService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database access.</param>
    public LeaseProposalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new lease proposal (rent a property). Requires the tenant to be verified.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant submitting the proposal.</param>
    /// <param name="dto">The lease proposal details.</param>
    /// <returns>A response DTO detailing the created lease proposal.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the tenant is not verified.</exception>
    public async Task<LeaseProposalResponseDto> CreateLeaseProposalAsync(Guid tenantId, CreateLeaseProposalDto dto)
    {
        var isVerified = await _unitOfWork.UserVerifications.IsUserVerifiedAsync(tenantId);
        if (!isVerified)
        {
            throw new InvalidOperationException("User must be verified to rent a property.");
        }

        var proposal = new LeaseProposal
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PropertyId = dto.PropertyId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MonthlyRent = dto.MonthlyRent,
            UpfrontPayment = dto.UpfrontPayment,
            SecurityDeposit = dto.SecurityDeposit,
            StatusId = 1, // Default status
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _unitOfWork.LeaseProposals.CreateAsync(proposal);
        await _unitOfWork.SaveChangesAsync();

        return new LeaseProposalResponseDto
        {
            Id = proposal.Id,
            TenantId = proposal.TenantId,
            PropertyId = proposal.PropertyId,
            StartDate = proposal.StartDate,
            EndDate = proposal.EndDate,
            MonthlyRent = proposal.MonthlyRent,
            UpfrontPayment = proposal.UpfrontPayment,
            SecurityDeposit = proposal.SecurityDeposit,
            StatusId = proposal.StatusId,
            CreatedAt = proposal.CreatedAt
        };
    }
}
