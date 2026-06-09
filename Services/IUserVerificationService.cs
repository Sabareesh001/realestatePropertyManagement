using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using propertyManagement.DTOs;

namespace propertyManagement.Services;

/// <summary>
/// Service interface for handling user verification workflows.
/// </summary>
public interface IUserVerificationService
{
    /// <summary>
    /// Submits a user verification request with verification documents.
    /// </summary>
    /// <param name="userId">The unique identifier of the user submitting verification.</param>
    /// <param name="submitVerificationDto">The submission data containing the list of documents.</param>
    /// <returns>A response DTO detailing the created verification request.</returns>
    Task<UserVerificationResponseDto> SubmitForVerificationAsync(Guid userId, SubmitVerificationDto submitVerificationDto);

    /// <summary>
    /// Approves a pending user verification request.
    /// </summary>
    /// <param name="adminId">The unique identifier of the administrator approving the request.</param>
    /// <param name="verificationId">The unique identifier of the verification request.</param>
    /// <param name="dto">The verification approval details containing optional remarks.</param>
    /// <returns>A response DTO detailing the updated verification request.</returns>
    Task<UserVerificationResponseDto> ApproveVerificationAsync(Guid adminId, Guid verificationId, VerifyRequestDto dto);

    /// <summary>
    /// Rejects a pending user verification request.
    /// </summary>
    /// <param name="adminId">The unique identifier of the administrator rejecting the request.</param>
    /// <param name="verificationId">The unique identifier of the verification request.</param>
    /// <param name="dto">The verification rejection details containing remarks/reasons.</param>
    /// <returns>A response DTO detailing the updated verification request.</returns>
    Task<UserVerificationResponseDto> RejectVerificationAsync(Guid adminId, Guid verificationId, VerifyRequestDto dto);

    /// <summary>
    /// Retrieves all user verification requests that are currently pending.
    /// </summary>
    /// <returns>A collection of pending user verification requests.</returns>
    Task<IEnumerable<UserVerificationResponseDto>> GetPendingVerificationsAsync();

    /// <summary>
    /// Gets the current verification status for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The verification status name (e.g. "Unverified", "Pending", "Verified", "Rejected").</returns>
    Task<string> GetVerificationStatusAsync(Guid userId);
}
