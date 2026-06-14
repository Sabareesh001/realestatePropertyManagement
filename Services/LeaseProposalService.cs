using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for lease proposal business operations.
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
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Creates a new lease proposal. Requires the tenant to be verified.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant submitting the proposal.</param>
    /// <param name="dto">The lease proposal details.</param>
    /// <returns>A response DTO detailing the created lease proposal.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the tenant is not verified or property is unavailable.</exception>
    public async Task<LeaseProposalResponseDto> CreateLeaseProposalAsync(Guid tenantId, CreateLeaseProposalDto dto)
    {
        var isVerified = await _unitOfWork.UserVerifications.IsUserVerifiedAsync(tenantId);
        if (!isVerified)
        {
            throw new InvalidOperationException("User must be verified to rent a property.");
        }

        var property = await _unitOfWork.Properties.GetByIdAsync(dto.PropertyId);
        if (property == null)
        {
            throw new KeyNotFoundException("Property not found.");
        }

        if (property.OwnerId == tenantId)
        {
            throw new InvalidOperationException("Owner cannot lease their own property.");
        }

        if (dto.StartDate.HasValue && dto.EndDate.HasValue)
        {
            var isOverlapping = await _unitOfWork.LeaseProposals.HasOverlappingProposalAsync(dto.PropertyId, dto.StartDate.Value, dto.EndDate.Value);
            if (isOverlapping)
            {
                throw new InvalidOperationException("A lease proposal already exists for this property during the specified time period.");
            }
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
            StatusId = ProposalStatus.Draft, // Default status for created rent request is Draft (1)
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _unitOfWork.LeaseProposals.CreateAsync(proposal);
        await _unitOfWork.SaveChangesAsync();

        // Fetch proposal with tenant info loaded to populate tenant details in response
        var createdProposal = await _unitOfWork.LeaseProposals.GetByIdAsync(proposal.Id);
        
        // Eager load tenant information manually if needed since GetByIdAsync might not include UserProfile/TenantProfile by default
        if (createdProposal != null)
        {
            createdProposal.Tenant = await _unitOfWork.Users.GetByIdAsync(tenantId);
        }

        return MapToResponseDto(createdProposal ?? proposal);
    }

    /// <summary>
    /// Retrieves all lease proposals submitted by a specific tenant.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A collection of lease proposal response DTOs.</returns>
    public async Task<IEnumerable<LeaseProposalResponseDto>> GetMyRequestsAsync(Guid tenantId)
    {
        var proposals = await _unitOfWork.LeaseProposals.GetProposalsByTenantIdAsync(tenantId);
        return proposals.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Retrieves all lease proposals received for properties owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <returns>A collection of lease proposal response DTOs containing tenant details.</returns>
    public async Task<IEnumerable<LeaseProposalResponseDto>> GetReceivedRequestsAsync(Guid ownerId)
    {
        var proposals = await _unitOfWork.LeaseProposals.GetProposalsByOwnerIdAsync(ownerId);
        return proposals.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Submits a draft lease proposal. Requires the tenant to be verified.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant submitting the proposal.</param>
    /// <param name="proposalId">The unique identifier of the lease proposal.</param>
    /// <returns>A response DTO detailing the submitted lease proposal.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the proposal is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not the tenant associated with the proposal.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the proposal is not in Draft status or tenant is unverified.</exception>
    public async Task<LeaseProposalResponseDto> SubmitLeaseProposalAsync(Guid tenantId, Guid proposalId)
    {
        var proposal = await _unitOfWork.LeaseProposals.GetByIdAsync(proposalId);
        if (proposal == null)
        {
            throw new KeyNotFoundException("Lease proposal not found.");
        }

        if (proposal.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("You are not authorized to submit this proposal.");
        }

        if (proposal.StatusId != ProposalStatus.Draft)
        {
            throw new InvalidOperationException("Only draft proposals can be submitted.");
        }

        var isVerified = await _unitOfWork.UserVerifications.IsUserVerifiedAsync(tenantId);
        if (!isVerified)
        {
            throw new InvalidOperationException("User must be verified to rent a property.");
        }

        proposal.StatusId = ProposalStatus.Submitted;
        proposal.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _unitOfWork.LeaseProposals.UpdateAsync(proposal);
        await _unitOfWork.SaveChangesAsync();

        // Eager load tenant information manually
        proposal.Tenant = await _unitOfWork.Users.GetByIdAsync(tenantId);

        return MapToResponseDto(proposal);
    }

    /// <summary>
    /// Reviews (approves or rejects) a submitted lease proposal. Called by the property owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner reviewing the proposal.</param>
    /// <param name="proposalId">The unique identifier of the lease proposal.</param>
    /// <param name="accept">True to approve the proposal; false to reject it.</param>
    /// <returns>A response DTO detailing the reviewed lease proposal.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the proposal or associated property is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not the owner of the property.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the proposal status is not Submitted.</exception>
    public async Task<LeaseProposalResponseDto> ReviewProposalAsync(Guid ownerId, Guid proposalId, bool accept)
    {
        var proposal = await _unitOfWork.LeaseProposals.GetByIdAsync(proposalId);
        if (proposal == null)
        {
            throw new KeyNotFoundException("Lease proposal not found.");
        }

        if (!proposal.PropertyId.HasValue)
        {
            throw new KeyNotFoundException("Property associated with the proposal not found.");
        }

        var property = await _unitOfWork.Properties.GetByIdAsync(proposal.PropertyId.Value);
        if (property == null)
        {
            throw new KeyNotFoundException("Property not found.");
        }

        if (property.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("You are not authorized to review this proposal.");
        }

        if (proposal.StatusId != ProposalStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted proposals can be reviewed.");
        }

        proposal.StatusId = accept ? ProposalStatus.Approved : ProposalStatus.Rejected;
        proposal.ReviewedBy = ownerId;
        proposal.ReviewedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
        proposal.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _unitOfWork.LeaseProposals.UpdateAsync(proposal);
        await _unitOfWork.SaveChangesAsync();

        if (proposal.TenantId.HasValue)
        {
            proposal.Tenant = await _unitOfWork.Users.GetByIdAsync(proposal.TenantId.Value);
        }

        return MapToResponseDto(proposal);
    }

    /// <summary>
    /// Cancels or withdraws a lease proposal. Called by the tenant.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <param name="proposalId">The unique identifier of the lease proposal.</param>
    /// <returns>A response DTO detailing the cancelled lease proposal.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the proposal is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not the tenant who created the proposal.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the proposal is not in Draft or Submitted status.</exception>
    public async Task<LeaseProposalResponseDto> CancelProposalAsync(Guid tenantId, Guid proposalId)
    {
        var proposal = await _unitOfWork.LeaseProposals.GetByIdAsync(proposalId);
        if (proposal == null)
        {
            throw new KeyNotFoundException("Lease proposal not found.");
        }

        if (proposal.TenantId != tenantId)
        {
            throw new UnauthorizedAccessException("You are not authorized to cancel this proposal.");
        }

        if (proposal.StatusId != ProposalStatus.Draft && proposal.StatusId != ProposalStatus.Submitted)
        {
            throw new InvalidOperationException("Only draft or submitted proposals can be cancelled.");
        }

        proposal.StatusId = ProposalStatus.Cancelled;
        proposal.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _unitOfWork.LeaseProposals.UpdateAsync(proposal);
        await _unitOfWork.SaveChangesAsync();

        proposal.Tenant = await _unitOfWork.Users.GetByIdAsync(tenantId);

        return MapToResponseDto(proposal);
    }

    /// <summary>
    /// Maps a LeaseProposal entity to a LeaseProposalResponseDto.
    /// </summary>
    private static LeaseProposalResponseDto MapToResponseDto(LeaseProposal proposal)
    {
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
            ReviewedBy = proposal.ReviewedBy,
            ReviewedAt = proposal.ReviewedAt,
            CreatedAt = proposal.CreatedAt,
            Tenant = MapToTenantDetailsDto(proposal.Tenant)
        };
    }

    /// <summary>
    /// Maps a User entity to TenantDetailsDto.
    /// </summary>
    private static TenantDetailsDto? MapToTenantDetailsDto(User? user)
    {
        if (user == null) return null;

        var tenantProfile = user.UserProfile?.TenantProfile;

        return new TenantDetailsDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Occupation = tenantProfile?.Occupation,
            MonthlyIncome = tenantProfile?.MonthlyIncome
        };
    }
}
