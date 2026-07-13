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
/// Service for managing complaints and their comment threads.
/// </summary>
public class ComplaintService : IComplaintService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of <see cref="ComplaintService"/>.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    public ComplaintService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc/>
    public async Task<ComplaintResponseDto> CreateComplaintAsync(Guid tenantId, CreateComplaintDto dto)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(dto.LeaseId);
        if (lease == null)
            throw new KeyNotFoundException("Lease not found.");

        if (lease.TenantId != tenantId)
            throw new UnauthorizedAccessException("You are not the tenant of this lease.");

        if (lease.StatusId != LeaseStatus.Active)
            throw new InvalidOperationException("Complaints can only be raised against an active lease.");

        var property = lease.PropertyNavigation
            ?? throw new InvalidOperationException("The lease is not associated with a property.");

        var complaint = new Complaint
        {
            LeaseId = dto.LeaseId,
            TenantId = tenantId,
            PropertyId = property.Id,
            OwnerId = property.OwnerId,
            ComplaintTypeId = dto.CategoryId,
            PriorityId = dto.PriorityId,
            StatusId = ComplaintStatus.Open,
            Subject = dto.Subject,
            Description = dto.Description,
            AttachmentUrl = dto.AttachmentUrl,
            CreatedBy = tenantId
        };

        await _unitOfWork.Complaints.CreateAsync(complaint);
        await _unitOfWork.SaveChangesAsync();

        var created = await _unitOfWork.Complaints.GetByIdAsync(complaint.Id);
        return MapToResponseDto(created!, includeComments: true, authorMap: new Dictionary<Guid, User>());
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<ComplaintResponseDto>> GetMyComplaintsAsync(Guid userId, PaginationParams pagination)
    {
        var complaints = await _unitOfWork.Complaints.GetByCreatedByAsync(userId, pagination.PageNumber, pagination.PageSize);
        return complaints.Select(c => MapToResponseDto(c, includeComments: false, authorMap: new Dictionary<Guid, User>()));
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<ComplaintResponseDto>> GetReceivedComplaintsAsync(Guid ownerId, PaginationParams pagination)
    {
        var complaints = await _unitOfWork.Complaints.GetByOwnerIdAsync(ownerId, pagination.PageNumber, pagination.PageSize);
        return complaints.Select(c => MapToResponseDto(c, includeComments: false, authorMap: new Dictionary<Guid, User>()));
    }

    /// <inheritdoc/>
    public async Task<PagedResultDto<ComplaintResponseDto>> GetAllComplaintsAsync(PaginationParams pagination)
    {
        var complaints = await _unitOfWork.Complaints.GetAllWithDetailsAsync(pagination.PageNumber, pagination.PageSize);
        return complaints.Select(c => MapToResponseDto(c, includeComments: false, authorMap: new Dictionary<Guid, User>()));
    }

    /// <inheritdoc/>
    public async Task<ComplaintResponseDto> GetComplaintByIdAsync(Guid id, Guid userId, IEnumerable<string> roles)
    {
        var complaint = await _unitOfWork.Complaints.GetByIdAsync(id);
        if (complaint == null)
            throw new KeyNotFoundException("Complaint not found.");

        EnsureParticipantOrAdmin(complaint, userId, roles);

        var authorMap = await BuildAuthorMapAsync(complaint.Comments.Select(c => c.AuthorId));
        return MapToResponseDto(complaint, includeComments: true, authorMap: authorMap);
    }

    /// <inheritdoc/>
    public async Task<ComplaintResponseDto> UpdateStatusAsync(
        Guid id, Guid userId, IEnumerable<string> roles, UpdateComplaintStatusDto dto)
    {
        var complaint = await _unitOfWork.Complaints.GetByIdAsync(id);
        if (complaint == null)
            throw new KeyNotFoundException("Complaint not found.");

        var roleList = roles.ToList();
        bool isAdmin = roleList.Contains("Admin");
        bool isOwner = roleList.Contains("Owner") && complaint.OwnerId == userId;
        bool isTenantCreator = roleList.Contains("Tenant") && complaint.CreatedBy == userId;

        ValidateStatusTransition(complaint.StatusId ?? 0, dto.StatusId, isAdmin, isOwner, isTenantCreator);

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            complaint.StatusId = dto.StatusId;

            if (dto.StatusId == ComplaintStatus.Resolved)
                complaint.ResolvedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            else if (dto.StatusId == ComplaintStatus.InProgress && complaint.ResolvedAt != null)
                complaint.ResolvedAt = null;

            await _unitOfWork.Complaints.UpdateAsync(complaint);

            if (!string.IsNullOrWhiteSpace(dto.Note))
            {
                var comment = new ComplaintComment
                {
                    ComplaintId = complaint.Id,
                    AuthorId = userId,
                    Message = dto.Note
                };
                await _unitOfWork.ComplaintComments.CreateAsync(comment);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        var updated = await _unitOfWork.Complaints.GetByIdAsync(complaint.Id);
        var authorMap = await BuildAuthorMapAsync(updated!.Comments.Select(c => c.AuthorId));
        return MapToResponseDto(updated, includeComments: true, authorMap: authorMap);
    }

    /// <inheritdoc/>
    public async Task<ComplaintCommentDto> AddCommentAsync(
        Guid id, Guid userId, IEnumerable<string> roles, AddCommentDto dto)
    {
        var complaint = await _unitOfWork.Complaints.GetByIdAsync(id);
        if (complaint == null)
            throw new KeyNotFoundException("Complaint not found.");

        EnsureParticipantOrAdmin(complaint, userId, roles);

        var activeStatuses = new[] { ComplaintStatus.Open, ComplaintStatus.InProgress, ComplaintStatus.Resolved };
        if (!activeStatuses.Contains(complaint.StatusId ?? 0))
            throw new InvalidOperationException("Comments cannot be added to a closed or cancelled complaint.");

        var comment = new ComplaintComment
        {
            ComplaintId = complaint.Id,
            AuthorId = userId,
            Message = dto.Message
        };
        await _unitOfWork.ComplaintComments.CreateAsync(comment);
        await _unitOfWork.SaveChangesAsync();

        var author = await _unitOfWork.Users.GetByIdAsync(userId);
        return MapToCommentDto(comment, complaint, author);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private static void EnsureParticipantOrAdmin(Complaint complaint, Guid userId, IEnumerable<string> roles)
    {
        var roleList = roles.ToList();
        bool isAdmin = roleList.Contains("Admin");
        bool isOwner = roleList.Contains("Owner") && complaint.OwnerId == userId;
        bool isTenant = roleList.Contains("Tenant") && complaint.TenantId == userId;

        if (!isAdmin && !isOwner && !isTenant)
            throw new UnauthorizedAccessException("You are not a participant of this complaint.");
    }

    private static void ValidateStatusTransition(
        int current, int next, bool isAdmin, bool isOwner, bool isTenantCreator)
    {
        // Terminal statuses — nothing can transition out of them
        if (current == ComplaintStatus.Closed || current == ComplaintStatus.Cancelled)
            throw new InvalidOperationException("This complaint is in a terminal status and cannot be updated.");

        bool ownerOrAdmin = isOwner || isAdmin;

        bool allowed = (current, next) switch
        {
            // Open → InProgress: owner/admin
            (ComplaintStatus.Open, ComplaintStatus.InProgress) => ownerOrAdmin,
            // Open|InProgress → Resolved: owner/admin
            (ComplaintStatus.Open, ComplaintStatus.Resolved) => ownerOrAdmin,
            (ComplaintStatus.InProgress, ComplaintStatus.Resolved) => ownerOrAdmin,
            // Open|InProgress → Cancelled: tenant-creator or admin
            (ComplaintStatus.Open, ComplaintStatus.Cancelled) => isTenantCreator || isAdmin,
            (ComplaintStatus.InProgress, ComplaintStatus.Cancelled) => isTenantCreator || isAdmin,
            // Resolved → Closed: tenant-creator
            (ComplaintStatus.Resolved, ComplaintStatus.Closed) => isTenantCreator,
            // Resolved → InProgress (reopen): tenant-creator or owner/admin
            (ComplaintStatus.Resolved, ComplaintStatus.InProgress) => isTenantCreator || ownerOrAdmin,
            _ => false
        };

        if (!allowed)
        {
            // Distinguish wrong-role (403) from invalid-transition (400)
            bool transitionExistsForSomeRole = (current, next) switch
            {
                (ComplaintStatus.Open, ComplaintStatus.InProgress) => true,
                (ComplaintStatus.Open, ComplaintStatus.Resolved) => true,
                (ComplaintStatus.InProgress, ComplaintStatus.Resolved) => true,
                (ComplaintStatus.Open, ComplaintStatus.Cancelled) => true,
                (ComplaintStatus.InProgress, ComplaintStatus.Cancelled) => true,
                (ComplaintStatus.Resolved, ComplaintStatus.Closed) => true,
                (ComplaintStatus.Resolved, ComplaintStatus.InProgress) => true,
                _ => false
            };

            if (transitionExistsForSomeRole)
                throw new UnauthorizedAccessException("You do not have permission to apply this status transition.");

            throw new InvalidOperationException($"Invalid status transition from {current} to {next}.");
        }
    }

    private async Task<Dictionary<Guid, User>> BuildAuthorMapAsync(IEnumerable<Guid> authorIds)
    {
        var distinctIds = authorIds.Distinct().ToList();
        var map = new Dictionary<Guid, User>();
        foreach (var aid in distinctIds)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(aid);
            if (user != null)
                map[aid] = user;
        }
        return map;
    }

    private static ComplaintResponseDto MapToResponseDto(
        Complaint complaint, bool includeComments, Dictionary<Guid, User> authorMap)
    {
        var dto = new ComplaintResponseDto
        {
            Id = complaint.Id,
            LeaseId = complaint.LeaseId,
            PropertyId = complaint.PropertyId,
            PropertyTitle = complaint.Property?.Title,
            TenantId = complaint.TenantId,
            TenantName = complaint.Tenant != null
                ? $"{complaint.Tenant.FirstName} {complaint.Tenant.LastName}"
                : null,
            OwnerId = complaint.OwnerId,
            CategoryId = complaint.ComplaintTypeId,
            CategoryName = complaint.ComplaintType?.Name,
            PriorityId = complaint.PriorityId,
            PriorityName = complaint.Priority?.Name,
            StatusId = complaint.StatusId,
            StatusName = complaint.Status?.Name,
            Subject = complaint.Subject,
            Description = complaint.Description,
            AttachmentUrl = complaint.AttachmentUrl,
            CreatedBy = complaint.CreatedBy,
            ResolvedAt = complaint.ResolvedAt,
            CreatedAt = complaint.CreatedAt,
            UpdatedAt = complaint.UpdatedAt,
            CommentCount = complaint.Comments.Count,
            Comments = new List<ComplaintCommentDto>()
        };

        if (includeComments)
        {
            dto.Comments = complaint.Comments
                .OrderBy(c => c.CreatedAt)
                .Select(c =>
                {
                    authorMap.TryGetValue(c.AuthorId, out var author);
                    return MapToCommentDto(c, complaint, author ?? c.Author);
                })
                .ToList();
        }

        return dto;
    }

    private static ComplaintCommentDto MapToCommentDto(ComplaintComment comment, Complaint complaint, User? author)
    {
        string? authorRole = null;
        if (author != null)
        {
            bool isAdmin = author.UserRoles.Any(ur => ur.RoleId == (int?)Role.Admin && ur.DeletedAt == null);
            bool isOwner = !isAdmin && author.Id == complaint.OwnerId;
            authorRole = isAdmin ? "Admin" : isOwner ? "Owner" : "Tenant";
        }

        return new ComplaintCommentDto
        {
            Id = comment.Id,
            ComplaintId = comment.ComplaintId,
            AuthorId = comment.AuthorId,
            AuthorName = author != null ? $"{author.FirstName} {author.LastName}" : null,
            AuthorRole = authorRole,
            Message = comment.Message,
            CreatedAt = comment.CreatedAt
        };
    }
}
