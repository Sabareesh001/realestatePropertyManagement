using System;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for lease proposal-related business operations.
/// </summary>
public interface ILeaseProposalService
{
    /// <summary>
    /// Creates a new lease proposal (rent a property). Requires the tenant to be verified.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant submitting the proposal.</param>
    /// <param name="dto">The lease proposal details.</param>
    /// <returns>A response DTO detailing the created lease proposal.</returns>
    Task<LeaseProposalResponseDto> CreateLeaseProposalAsync(Guid tenantId, CreateLeaseProposalDto dto);
}
