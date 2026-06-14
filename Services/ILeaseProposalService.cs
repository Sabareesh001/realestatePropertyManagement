using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for lease proposal business operations.
/// </summary>
public interface ILeaseProposalService
{
    /// <summary>
    /// Creates a new lease proposal. Requires the tenant to be verified.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant submitting the proposal.</param>
    /// <param name="dto">The lease proposal details.</param>
    /// <returns>A response DTO detailing the created lease proposal.</returns>
    Task<LeaseProposalResponseDto> CreateLeaseProposalAsync(Guid tenantId, CreateLeaseProposalDto dto);

    /// <summary>
    /// Retrieves all lease proposals submitted by a specific tenant.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <returns>A collection of lease proposal response DTOs.</returns>
    Task<IEnumerable<LeaseProposalResponseDto>> GetMyRequestsAsync(Guid tenantId);

    /// <summary>
    /// Retrieves all lease proposals received for properties owned by a specific owner.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <returns>A collection of lease proposal response DTOs containing tenant details.</returns>
    Task<IEnumerable<LeaseProposalResponseDto>> GetReceivedRequestsAsync(Guid ownerId);

    /// <summary>
    /// Submits a draft lease proposal. Requires the tenant to be verified.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant submitting the proposal.</param>
    /// <param name="proposalId">The unique identifier of the lease proposal.</param>
    /// <returns>A response DTO detailing the submitted lease proposal.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the proposal is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not the tenant associated with the proposal.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the proposal is not in Draft status or tenant is unverified.</exception>
    Task<LeaseProposalResponseDto> SubmitLeaseProposalAsync(Guid tenantId, Guid proposalId);

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
    Task<LeaseProposalResponseDto> ReviewProposalAsync(Guid ownerId, Guid proposalId, bool accept);

    /// <summary>
    /// Cancels or withdraws a lease proposal. Called by the tenant.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant.</param>
    /// <param name="proposalId">The unique identifier of the lease proposal.</param>
    /// <returns>A response DTO detailing the cancelled lease proposal.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the proposal is not found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not the tenant who created the proposal.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the proposal is not in Draft or Submitted status.</exception>
    Task<LeaseProposalResponseDto> CancelProposalAsync(Guid tenantId, Guid proposalId);
}
