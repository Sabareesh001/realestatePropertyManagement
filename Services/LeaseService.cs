using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for Lease-related business operations.
/// </summary>
public class LeaseService : ILeaseService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database operations.</param>
    public LeaseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Creates a new lease in Draft or Submitted status (called by the owner).
    /// </summary>
    public async Task<LeaseResponseDto> CreateLeaseAsync(Guid ownerId, CreateLeaseDto dto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(dto.PropertyId);
        if (property == null || property.OwnerId != ownerId)
        {
            throw new InvalidOperationException("Property not found or you are not the owner.");
        }

        if (property.OwnerId == dto.TenantId)
        {
            throw new InvalidOperationException("Owner cannot lease their own property.");
        }

        var tenant = await _unitOfWork.Users.GetByIdAsync(dto.TenantId);
        if (tenant == null)
        {
            throw new InvalidOperationException("Tenant not found.");
        }

        var isVerified = await _unitOfWork.UserVerifications.IsUserVerifiedAsync(dto.TenantId);
        if (!isVerified)
        {
            throw new InvalidOperationException("Tenant must be verified to rent a property.");
        }

        var proposal = await _unitOfWork.LeaseProposals.GetByIdAsync(dto.ProposalId);
        if (proposal == null)
        {
            throw new InvalidOperationException("Lease proposal not found.");
        }

        if (proposal.PropertyId != dto.PropertyId || proposal.TenantId != dto.TenantId)
        {
            throw new InvalidOperationException("Lease proposal does not match the specified property or tenant.");
        }

        var lease = new Lease
        {
            Id = Guid.NewGuid(),
            TenantId = dto.TenantId,
            PropertyId = dto.PropertyId,
            ProposalId = dto.ProposalId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MonthlyRent = dto.MonthlyRent,
            UpfrontPayment = dto.UpfrontPayment,
            SecurityDeposit = dto.SecurityDeposit,
            StatusId = LeaseStatus.Draft,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        if (!string.IsNullOrEmpty(dto.AgreementDocumentUrl))
        {
            lease.AgreementDocument = new Document
            {
                Id = Guid.NewGuid(),
                DocumentTypeId = DocumentType.LeaseAgreement,
                DocumentUrl = dto.AgreementDocumentUrl,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };
        }

        if (dto.Documents != null)
        {
            foreach (var docDto in dto.Documents)
            {
                lease.Documents.Add(new Document
                {
                    Id = Guid.NewGuid(),
                    DocumentTypeId = docDto.DocumentTypeId,
                    DocumentNumber = docDto.DocumentNumber,
                    DocumentUrl = docDto.DocumentUrl,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                });
            }
        }

        await _unitOfWork.Leases.CreateAsync(lease);
        await _unitOfWork.SaveChangesAsync();

        var createdLease = await _unitOfWork.Leases.GetByIdAsync(lease.Id);
        return MapToResponseDto(createdLease ?? lease);
    }

    /// <summary>
    /// Updates an existing lease draft (called by the owner).
    /// </summary>
    public async Task<LeaseResponseDto> UpdateLeaseAsync(Guid ownerId, Guid leaseId, UpdateLeaseDto dto)
    {
        var lease = await _unitOfWork.Leases.GetByIdWithDocumentsAsync(leaseId);
        if (lease == null || lease.PropertyNavigation?.OwnerId != ownerId)
        {
            throw new InvalidOperationException("Lease not found or you are not the owner of the property.");
        }

        if (lease.StatusId != LeaseStatus.Draft && lease.StatusId != LeaseStatus.Submitted)
        {
            throw new InvalidOperationException("Lease can only be updated when in Draft or Submitted status.");
        }

        if (dto.StartDate.HasValue) lease.StartDate = dto.StartDate.Value;
        if (dto.EndDate.HasValue) lease.EndDate = dto.EndDate.Value;
        if (dto.MonthlyRent.HasValue) lease.MonthlyRent = dto.MonthlyRent.Value;
        if (dto.UpfrontPayment.HasValue) lease.UpfrontPayment = dto.UpfrontPayment.Value;
        if (dto.SecurityDeposit.HasValue) lease.SecurityDeposit = dto.SecurityDeposit.Value;
        if (dto.StatusId.HasValue)
        {
            if (dto.StatusId.Value == LeaseStatus.Submitted)
            {
                throw new InvalidOperationException("Please use the submit endpoint to submit the lease.");
            }
            lease.StatusId = dto.StatusId.Value;
        }

        if (!string.IsNullOrEmpty(dto.AgreementDocumentUrl))
        {
            if (lease.AgreementDocument != null)
            {
                lease.AgreementDocument.DocumentUrl = dto.AgreementDocumentUrl;
                lease.AgreementDocument.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
            }
            else
            {
                var newDoc = new Document
                {
                    Id = Guid.NewGuid(),
                    DocumentTypeId = DocumentType.LeaseAgreement,
                    DocumentUrl = dto.AgreementDocumentUrl,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };
                lease.AgreementDocument = newDoc;
                await _unitOfWork.Documents.CreateAsync(newDoc);
            }
        }

        if (dto.Documents != null)
        {
            lease.Documents.Clear();
            foreach (var docDto in dto.Documents)
            {
                var newDoc = new Document
                {
                    Id = Guid.NewGuid(),
                    DocumentTypeId = docDto.DocumentTypeId,
                    DocumentNumber = docDto.DocumentNumber,
                    DocumentUrl = docDto.DocumentUrl,
                    CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                };
                lease.Documents.Add(newDoc);
                await _unitOfWork.Documents.CreateAsync(newDoc);
            }
        }

        await _unitOfWork.Leases.UpdateAsync(lease);
        await _unitOfWork.SaveChangesAsync();

        var updatedLease = await _unitOfWork.Leases.GetByIdAsync(lease.Id);
        return MapToResponseDto(updatedLease ?? lease);
    }

    /// <summary>
    /// Verifies/approves or rejects a submitted lease agreement template (called by an admin).
    /// </summary>
    public async Task<LeaseResponseDto> VerifyLeaseTemplateAsync(Guid adminId, Guid leaseId, bool approve)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        if (lease.StatusId != LeaseStatus.Submitted)
        {
            throw new InvalidOperationException("Lease template must be in Submitted status to be verified.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (approve)
            {
                lease.StatusId = LeaseStatus.PendingSignature;

                if (lease.ProposalId != Guid.Empty)
                {
                    var proposal = await _unitOfWork.LeaseProposals.GetByIdAsync(lease.ProposalId);
                    if (proposal != null)
                    {
                        proposal.StatusId = ProposalStatus.Approved;
                        proposal.ReviewedBy = adminId;
                        proposal.ReviewedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                        await _unitOfWork.LeaseProposals.UpdateAsync(proposal);
                    }
                }
            }
            else
            {
                lease.StatusId = LeaseStatus.Rejected;

                if (lease.ProposalId != Guid.Empty)
                {
                    var proposal = await _unitOfWork.LeaseProposals.GetByIdAsync(lease.ProposalId);
                    if (proposal != null)
                    {
                        proposal.StatusId = ProposalStatus.Rejected;
                        proposal.ReviewedBy = adminId;
                        proposal.ReviewedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                        await _unitOfWork.LeaseProposals.UpdateAsync(proposal);
                    }
                }
            }

            await _unitOfWork.Leases.UpdateAsync(lease);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        var updatedLease = await _unitOfWork.Leases.GetByIdAsync(lease.Id);
        return MapToResponseDto(updatedLease ?? lease);
    }

    /// <summary>
    /// Signs the lease and uploads the signed agreement document (called by the tenant).
    /// </summary>
    public async Task<LeaseResponseDto> SignLeaseAsync(Guid tenantId, Guid leaseId, SignLeaseDto dto)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null || lease.TenantId != tenantId)
        {
            throw new InvalidOperationException("Lease not found or you are not the tenant associated with it.");
        }

        if (lease.StatusId != LeaseStatus.PendingSignature)
        {
            throw new InvalidOperationException("Lease is not ready for signature.");
        }

        if (string.IsNullOrEmpty(dto.SignedAgreementDocumentUrl))
        {
            throw new InvalidOperationException("Signed agreement document URL is mandatory.");
        }

        var signedDoc = new Document
        {
            Id = Guid.NewGuid(),
            DocumentTypeId = DocumentType.SignedLeaseAgreement,
            DocumentUrl = dto.SignedAgreementDocumentUrl,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };
        lease.SignedAgreementDocument = signedDoc;
        await _unitOfWork.Documents.CreateAsync(signedDoc);

        lease.StatusId = LeaseStatus.TenantSigned;

        await _unitOfWork.Leases.UpdateAsync(lease);
        await _unitOfWork.SaveChangesAsync();

        var updatedLease = await _unitOfWork.Leases.GetByIdAsync(lease.Id);
        return MapToResponseDto(updatedLease ?? lease);
    }

    /// <summary>
    /// Verifies/approves or rejects a tenant-signed lease agreement (called by an admin) to make it active.
    /// </summary>
    public async Task<LeaseResponseDto> VerifySignedLeaseAsync(Guid adminId, Guid leaseId, bool approve)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        if (lease.StatusId != LeaseStatus.TenantSigned)
        {
            throw new InvalidOperationException("Lease must be signed by the tenant to be verified.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (approve)
            {
                lease.StatusId = LeaseStatus.Active;

                if (lease.PropertyId.HasValue)
                {
                    var property = await _unitOfWork.Properties.GetByIdAsync(lease.PropertyId.Value);
                    if (property != null)
                    {
                        property.AvailabilityStatusId = PropertyAvailabilityStatus.Occupied;
                        await _unitOfWork.Properties.UpdateAsync(property);
                    }
                }
            }
            else
            {
                lease.StatusId = LeaseStatus.Rejected;
            }

            await _unitOfWork.Leases.UpdateAsync(lease);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        var updatedLease = await _unitOfWork.Leases.GetByIdAsync(lease.Id);
        return MapToResponseDto(updatedLease ?? lease);
    }

    /// <summary>
    /// Retrieves a specific lease details, validating role-based authorization.
    /// </summary>
    public async Task<LeaseResponseDto> GetLeaseByIdAsync(Guid leaseId, Guid userId, IEnumerable<string> roles)
    {
        var lease = await _unitOfWork.Leases.GetByIdAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        if (roles.Contains("Admin"))
        {
            return MapToResponseDto(lease);
        }

        if (roles.Contains("Owner") && lease.PropertyNavigation?.OwnerId == userId)
        {
            return MapToResponseDto(lease);
        }

        if (roles.Contains("Tenant") && lease.TenantId == userId)
        {
            // Tenant cannot see draft or submitted leases (only pending signature, signed, active, etc.)
            if (lease.StatusId == LeaseStatus.Draft || lease.StatusId == LeaseStatus.Submitted)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this lease in its current state.");
            }
            return MapToResponseDto(lease);
        }

        throw new UnauthorizedAccessException("You are not authorized to view this lease.");
    }

    /// <summary>
    /// Retrieves all leases associated with the user based on their role.
    /// </summary>
    public async Task<IEnumerable<LeaseResponseDto>> GetMyLeasesAsync(Guid userId, IEnumerable<string> roles)
    {
        if (roles.Contains("Admin"))
        {
            var leases = await _unitOfWork.Leases.GetAllAsync();
            return leases.Select(MapToResponseDto).ToList();
        }

        var allLeases = await _unitOfWork.Leases.GetAllAsync();
        var result = new List<Lease>();

        if (roles.Contains("Owner"))
        {
            var ownerLeases = allLeases.Where(l => l.PropertyNavigation?.OwnerId == userId);
            result.AddRange(ownerLeases);
        }

        if (roles.Contains("Tenant"))
        {
            var tenantLeases = allLeases.Where(l => l.TenantId == userId && l.StatusId != LeaseStatus.Draft && l.StatusId != LeaseStatus.Submitted);
            result.AddRange(tenantLeases);
        }

        var distinctLeases = result.GroupBy(l => l.Id).Select(g => g.First());
        return distinctLeases.Select(MapToResponseDto).ToList();
    }

    /// <summary>
    /// Maps a Lease entity to a LeaseResponseDto.
    /// </summary>
    private static LeaseResponseDto MapToResponseDto(Lease lease)
    {
        return new LeaseResponseDto
        {
            Id = lease.Id,
            TenantId = lease.TenantId,
            PropertyId = lease.PropertyId,
            ProposalId = lease.ProposalId,
            StartDate = lease.StartDate,
            EndDate = lease.EndDate,
            MonthlyRent = lease.MonthlyRent,
            UpfrontPayment = lease.UpfrontPayment,
            SecurityDeposit = lease.SecurityDeposit,
            StatusId = lease.StatusId,
            StatusName = lease.Status?.Name,
            AgreementDocumentUrl = lease.AgreementDocument != null && lease.AgreementDocument.DeletedAt == null ? lease.AgreementDocument.DocumentUrl : null,
            SignedAgreementDocumentUrl = lease.SignedAgreementDocument != null && lease.SignedAgreementDocument.DeletedAt == null ? lease.SignedAgreementDocument.DocumentUrl : null,
            CreatedAt = lease.CreatedAt,
            UpdatedAt = lease.UpdatedAt
        };
    }

    /// <summary>
    /// Retrieves all additional documents associated with a specific lease.
    /// </summary>
    public async Task<IEnumerable<DocumentResponseDto>> GetLeaseDocumentsAsync(Guid leaseId, Guid userId, IEnumerable<string> roles)
    {
        var lease = await _unitOfWork.Leases.GetByIdWithDocumentsAsync(leaseId);
        if (lease == null)
        {
            throw new KeyNotFoundException("Lease not found.");
        }

        bool isAuthorized = false;

        if (roles.Contains("Admin"))
        {
            isAuthorized = true;
        }
        else if (roles.Contains("Owner") && lease.PropertyNavigation?.OwnerId == userId)
        {
            isAuthorized = true;
        }
        else if (roles.Contains("Tenant") && lease.TenantId == userId)
        {
            // Tenant cannot see documents of draft or submitted leases
            if (lease.StatusId == LeaseStatus.Draft || lease.StatusId == LeaseStatus.Submitted)
            {
                throw new UnauthorizedAccessException("You are not authorized to view documents for this lease in its current state.");
            }
            isAuthorized = true;
        }

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to view documents for this lease.");
        }

        var result = new List<DocumentResponseDto>();

        if (lease.AgreementDocument != null && lease.AgreementDocument.DeletedAt == null)
        {
            result.Add(new DocumentResponseDto
            {
                Id = lease.AgreementDocument.Id,
                DocumentTypeId = lease.AgreementDocument.DocumentTypeId,
                DocumentNumber = lease.AgreementDocument.DocumentNumber,
                DocumentUrl = lease.AgreementDocument.DocumentUrl
            });
        }

        if (lease.SignedAgreementDocument != null && lease.SignedAgreementDocument.DeletedAt == null)
        {
            result.Add(new DocumentResponseDto
            {
                Id = lease.SignedAgreementDocument.Id,
                DocumentTypeId = lease.SignedAgreementDocument.DocumentTypeId,
                DocumentNumber = lease.SignedAgreementDocument.DocumentNumber,
                DocumentUrl = lease.SignedAgreementDocument.DocumentUrl
            });
        }

        if (lease.Documents != null)
        {
            result.AddRange(lease.Documents
                .Where(d => d.DeletedAt == null)
                .Select(d => new DocumentResponseDto
                {
                    Id = d.Id,
                    DocumentTypeId = d.DocumentTypeId,
                    DocumentNumber = d.DocumentNumber,
                    DocumentUrl = d.DocumentUrl
                }));
        }

        return result.GroupBy(d => d.Id).Select(g => g.First()).ToList();
    }

    /// <summary>
    /// Submits a lease template for verification (called by the owner).
    /// </summary>
    /// <param name="ownerId">The identifier of the owner submitting the lease.</param>
    /// <param name="leaseId">The identifier of the lease to submit.</param>
    /// <returns>A response DTO representing the submitted lease.</returns>
    /// <exception cref="InvalidOperationException">Thrown when lease is not found, owner mismatch, lease not in Draft, or agreement document is missing.</exception>
    public async Task<LeaseResponseDto> SubmitLeaseAsync(Guid ownerId, Guid leaseId)
    {
        var lease = await _unitOfWork.Leases.GetByIdWithDocumentsAsync(leaseId);
        if (lease == null || lease.PropertyNavigation?.OwnerId != ownerId)
        {
            throw new InvalidOperationException("Lease not found or you are not the owner of the property.");
        }

        if (lease.StatusId != LeaseStatus.Draft)
        {
            throw new InvalidOperationException("Only leases in Draft status can be submitted.");
        }

        if (lease.AgreementDocument == null || lease.AgreementDocument.DeletedAt != null || string.IsNullOrEmpty(lease.AgreementDocument.DocumentUrl))
        {
            throw new InvalidOperationException("Please upload an agreement document before submitting the lease.");
        }

        lease.StatusId = LeaseStatus.Submitted;

        await _unitOfWork.Leases.UpdateAsync(lease);
        await _unitOfWork.SaveChangesAsync();

        var updatedLease = await _unitOfWork.Leases.GetByIdAsync(lease.Id);
        return MapToResponseDto(updatedLease ?? lease);
    }
}
