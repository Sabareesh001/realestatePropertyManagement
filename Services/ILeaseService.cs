using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for Lease-related business operations.
/// </summary>
public interface ILeaseService
{
    /// <summary>
    /// Creates a new lease in Draft or Submitted status (called by the owner).
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner creating the lease.</param>
    /// <param name="dto">The lease creation details.</param>
    /// <returns>A response DTO representing the created lease.</returns>
    Task<LeaseResponseDto> CreateLeaseAsync(Guid ownerId, CreateLeaseDto dto);

    /// <summary>
    /// Updates an existing lease draft (called by the owner).
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <param name="leaseId">The unique identifier of the lease to update.</param>
    /// <param name="dto">The lease update details.</param>
    /// <returns>A response DTO representing the updated lease.</returns>
    Task<LeaseResponseDto> UpdateLeaseAsync(Guid ownerId, Guid leaseId, UpdateLeaseDto dto);

    /// <summary>
    /// Submits a lease template for verification (called by the owner).
    /// </summary>
    /// <param name="ownerId">The identifier of the owner submitting the lease.</param>
    /// <param name="leaseId">The identifier of the lease to submit.</param>
    /// <returns>A response DTO representing the submitted lease.</returns>
    Task<LeaseResponseDto> SubmitLeaseAsync(Guid ownerId, Guid leaseId);

    /// <summary>
    /// Verifies/approves or rejects a submitted lease agreement template (called by an admin).
    /// </summary>
    /// <param name="adminId">The unique identifier of the admin verifying the lease.</param>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="approve">True to approve the template; false to reject it.</param>
    /// <returns>A response DTO representing the updated lease state.</returns>
    Task<LeaseResponseDto> VerifyLeaseTemplateAsync(Guid adminId, Guid leaseId, bool approve);

    /// <summary>
    /// Signs the lease and uploads the signed agreement document (called by the tenant).
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant signing the lease.</param>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="dto">The lease signature details.</param>
    /// <returns>A response DTO representing the signed lease.</returns>
    Task<LeaseResponseDto> SignLeaseAsync(Guid tenantId, Guid leaseId, SignLeaseDto dto);

    /// <summary>
    /// Verifies/approves or rejects a tenant-signed lease agreement (called by an admin) to make it active.
    /// </summary>
    /// <param name="adminId">The unique identifier of the admin verifying the signed lease.</param>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="approve">True to activate the lease; false to reject it.</param>
    /// <returns>A response DTO representing the activated lease state.</returns>
    Task<LeaseResponseDto> VerifySignedLeaseAsync(Guid adminId, Guid leaseId, bool approve);

    /// <summary>
    /// Retrieves a specific lease details, validating role-based authorization.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="userId">The unique identifier of the user requesting details.</param>
    /// <param name="roles">The roles of the requesting user (Tenant, Owner, Admin).</param>
    /// <returns>A response DTO representing the lease.</returns>
    Task<LeaseResponseDto> GetLeaseByIdAsync(Guid leaseId, Guid userId, IEnumerable<string> roles);

    /// <summary>
    /// Retrieves all leases associated with the user based on their roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roles">The roles of the user (Tenant, Owner, Admin).</param>
    /// <returns>A collection of lease response DTOs.</returns>
    Task<IEnumerable<LeaseResponseDto>> GetMyLeasesAsync(Guid userId, IEnumerable<string> roles);

    /// <summary>
    /// Retrieves all additional documents associated with a specific lease.
    /// </summary>
    /// <param name="leaseId">The unique identifier of the lease.</param>
    /// <param name="userId">The unique identifier of the user requesting documents.</param>
    /// <param name="roles">The roles of the requesting user.</param>
    /// <returns>A collection of document response DTOs.</returns>
    Task<IEnumerable<DocumentResponseDto>> GetLeaseDocumentsAsync(Guid leaseId, Guid userId, IEnumerable<string> roles);
}
