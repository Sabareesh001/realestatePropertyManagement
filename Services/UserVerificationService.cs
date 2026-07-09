using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Extensions;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for handling user verification workflows.
/// </summary>
public class UserVerificationService : IUserVerificationService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserVerificationService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    public UserVerificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Submits a user verification request with verification documents.
    /// </summary>
    /// <param name="userId">The unique identifier of the user submitting verification.</param>
    /// <param name="submitVerificationDto">The submission data containing the list of documents.</param>
    /// <returns>A response DTO detailing the created verification request.</returns>
    /// <exception cref="ArgumentNullException">Thrown when submitVerificationDto is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if user has a pending request or is already verified.</exception>
    public async Task<UserVerificationResponseDto> SubmitForVerificationAsync(Guid userId, SubmitVerificationDto submitVerificationDto)
    {
        if (submitVerificationDto == null)
        {
            throw new ArgumentNullException(nameof(submitVerificationDto));
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var latestVerification = await _unitOfWork.UserVerifications.GetLatestVerificationByUserIdAsync(userId);
        if (latestVerification != null)
        {
            if (latestVerification.Status == UserVerification.StatusPending)
            {
                throw new InvalidOperationException("A verification request is already pending.");
            }
            if (latestVerification.Status == UserVerification.StatusVerified)
            {
                throw new InvalidOperationException("User is already verified.");
            }
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var userVerification = new UserVerification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = UserVerification.StatusPending,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow,DateTimeKind.Unspecified)
            };

            user.VerificationStatusId = UserVerificationStatus.Pending;
            await _unitOfWork.Users.UpdateAsync(user);

            foreach (var docDto in submitVerificationDto.Documents)
            {
                var doc = new Document
                {
                    Id = Guid.NewGuid(),
                    DocumentTypeId = docDto.DocumentTypeId,
                    DocumentNumber = docDto.DocumentNumber,
                    DocumentUrl = docDto.DocumentUrl,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };

                userVerification.UserVerificationDocuments.Add(new UserVerificationDocument
                {
                    UserVerificationId = userVerification.Id,
                    Document = doc
                });
            }

            await _unitOfWork.UserVerifications.CreateAsync(userVerification);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return MapToResponseDto(userVerification);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <summary>
    /// Approves a pending user verification request.
    /// </summary>
    /// <param name="adminId">The unique identifier of the administrator approving the request.</param>
    /// <param name="verificationId">The unique identifier of the verification request.</param>
    /// <param name="dto">The verification approval details containing optional remarks.</param>
    /// <returns>A response DTO detailing the updated verification request.</returns>
    /// <exception cref="InvalidOperationException">Thrown if verification request is not found or not in pending state.</exception>
    public async Task<UserVerificationResponseDto> ApproveVerificationAsync(Guid adminId, Guid verificationId, VerifyRequestDto dto)
    {
        var verification = await _unitOfWork.UserVerifications.GetByIdAsync(verificationId);
        if (verification == null)
        {
            throw new InvalidOperationException("Verification request not found.");
        }

        if (verification.Status != UserVerification.StatusPending)
        {
            throw new InvalidOperationException($"Cannot approve a request with status '{verification.Status}'.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            verification.Status = UserVerification.StatusVerified;
            verification.VerifiedBy = adminId;
            verification.Remarks = dto?.Remarks;
            verification.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _unitOfWork.UserVerifications.UpdateAsync(verification);

            var user = await _unitOfWork.Users.GetByIdAsync(verification.UserId);
            if (user != null)
            {
                user.VerificationStatusId = UserVerificationStatus.Verified;
                await _unitOfWork.Users.UpdateAsync(user);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return MapToResponseDto(verification);
    }

    /// <summary>
    /// Rejects a pending user verification request.
    /// </summary>
    /// <param name="adminId">The unique identifier of the administrator rejecting the request.</param>
    /// <param name="verificationId">The unique identifier of the verification request.</param>
    /// <param name="dto">The verification rejection details containing remarks/reasons.</param>
    /// <returns>A response DTO detailing the updated verification request.</returns>
    /// <exception cref="InvalidOperationException">Thrown if verification request is not found or not in pending state.</exception>
    public async Task<UserVerificationResponseDto> RejectVerificationAsync(Guid adminId, Guid verificationId, VerifyRequestDto dto)
    {
        var verification = await _unitOfWork.UserVerifications.GetByIdAsync(verificationId);
        if (verification == null)
        {
            throw new InvalidOperationException("Verification request not found.");
        }

        if (verification.Status != UserVerification.StatusPending)
        {
            throw new InvalidOperationException($"Cannot reject a request with status '{verification.Status}'.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            verification.Status = UserVerification.StatusRejected;
            verification.VerifiedBy = adminId;
            verification.Remarks = dto?.Remarks ?? "Rejected by Admin";
            verification.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            await _unitOfWork.UserVerifications.UpdateAsync(verification);

            var user = await _unitOfWork.Users.GetByIdAsync(verification.UserId);
            if (user != null)
            {
                user.VerificationStatusId = UserVerificationStatus.Rejected;
                await _unitOfWork.Users.UpdateAsync(user);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return MapToResponseDto(verification);
    }

    /// <summary>
    /// Retrieves all user verification requests that are currently pending.
    /// </summary>
    /// <returns>A collection of pending user verification requests.</returns>
    public async Task<PagedResultDto<UserVerificationResponseDto>> GetPendingVerificationsAsync(PaginationParams pagination)
    {
        var verifications = await _unitOfWork.UserVerifications.GetPendingVerificationsAsync(pagination.PageNumber, pagination.PageSize);
        return verifications.Select(MapToResponseDto);
    }

    /// <summary>
    /// Gets the current verification status for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>The verification status name (e.g. "Unverified", "Pending", "Verified", "Rejected").</returns>
    public async Task<string> GetVerificationStatusAsync(Guid userId)
    {
        var latest = await _unitOfWork.UserVerifications.GetLatestVerificationByUserIdAsync(userId);
        return latest?.Status ?? UserVerification.StatusUnverified;
    }

    private UserVerificationResponseDto MapToResponseDto(UserVerification uv)
    {
        return new UserVerificationResponseDto
        {
            Id = uv.Id,
            UserId = uv.UserId,
            Status = uv.Status,
            Remarks = uv.Remarks,
            VerifiedBy = uv.VerifiedBy,
            CreatedAt = uv.CreatedAt,
            UpdatedAt = uv.UpdatedAt,
            Documents = uv.UserVerificationDocuments.Select(uvd => new DocumentResponseDto
            {
                Id = uvd.DocumentId,
                DocumentTypeId = uvd.Document?.DocumentTypeId,
                DocumentNumber = uvd.Document?.DocumentNumber,
                DocumentUrl = uvd.Document?.DocumentUrl
            }).ToList()
        };
    }
}
