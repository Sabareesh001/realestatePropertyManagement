using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using propertyManagement.DTOs;
using propertyManagement.Models;
using propertyManagement.Repositories;

namespace propertyManagement.Services;

/// <summary>
/// Service implementation for property-related business operations.
/// </summary>
public class PropertyService : IPropertyService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for database access.</param>
    public PropertyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<PropertyResponseDto> CreatePropertyAsync(Guid ownerId, CreatePropertyDto dto)
    {
        var isVerified = await _unitOfWork.UserVerifications.IsUserVerifiedAsync(ownerId);
        if (!isVerified)
        {
            throw new InvalidOperationException("User must be verified to post a property.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var property = new Property
            {
                OwnerId = ownerId,
                Title = dto.Title,
                Description = dto.Description,
                AddressLine = dto.AddressLine,
                CityId = dto.CityId,
                MonthlyRent = dto.MonthlyRent,
                UpfrontPayment = dto.UpfrontPayment,
                SecurityDeposit = dto.SecurityDeposit,
                ThumbnailImgUrl = dto.ThumbnailImgUrl,
                VisitPreferences = dto.VisitPreferences,
                SpecificVisitDays = dto.SpecificVisitDays,
                VisitStartTime = dto.VisitStartTime,
                VisitEndTime = dto.VisitEndTime,
                VerificationStatusId = PropertyVerificationStatus.Draft,
                AvailabilityStatusId = PropertyAvailabilityStatus.Unavailable,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            if (dto.PropertyImages != null)
            {
                foreach (var imgDto in dto.PropertyImages)
                {
                    property.PropertyImages.Add(new PropertyImage
                    {
                        ImageUrl = imgDto.ImageUrl,
                        Description = imgDto.Description,
                        DisplayOrder = imgDto.DisplayOrder,
                        CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                    });
                }
            }

            await _unitOfWork.Properties.CreateAsync(property);
            await _unitOfWork.CommitTransactionAsync();

            return MapToResponseDto(property);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PropertyResponseDto> GetPropertyByIdAsync(int id, Guid? requestingUserId = null)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
            throw new KeyNotFoundException("Property not found.");

        var isVerified = property.VerificationStatusId == PropertyVerificationStatus.Verified;
        var isOwner = requestingUserId.HasValue && property.OwnerId == requestingUserId.Value;
        var isAdmin = requestingUserId.HasValue && await _unitOfWork.Users.HasRoleAsync(requestingUserId.Value, Role.Admin);

        if (!isVerified && !isOwner && !isAdmin)
            throw new KeyNotFoundException("Property not found.");

        return MapToResponseDto(property);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync()
    {
        var properties = await _unitOfWork.Properties.GetAllAsync();
        return properties
            .Where(p => p.VerificationStatusId == PropertyVerificationStatus.Verified)
            .Select(MapToResponseDto);
    }

    /// <inheritdoc />
    public async Task<PropertyResponseDto> UpdatePropertyAsync(Guid userId, int id, UpdatePropertyDto dto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
        {
            throw new KeyNotFoundException("Property not found.");
        }

        if (property.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this property.");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            property.Title = dto.Title;
            property.Description = dto.Description;
            property.AddressLine = dto.AddressLine;
            property.CityId = dto.CityId;
            property.MonthlyRent = dto.MonthlyRent;
            property.UpfrontPayment = dto.UpfrontPayment;
            property.SecurityDeposit = dto.SecurityDeposit;
            property.ThumbnailImgUrl = dto.ThumbnailImgUrl;
            property.VisitPreferences = dto.VisitPreferences;
            property.SpecificVisitDays = dto.SpecificVisitDays;
            property.VisitStartTime = dto.VisitStartTime;
            property.VisitEndTime = dto.VisitEndTime;
            property.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

            if (dto.PropertyImages != null)
            {
                // Remove images not present in the new list
                var newImageIds = dto.PropertyImages.Where(img => img.Id.HasValue).Select(img => img.Id!.Value).ToHashSet();
                var imagesToRemove = property.PropertyImages.Where(pi => !newImageIds.Contains(pi.Id)).ToList();
                foreach (var img in imagesToRemove)
                {
                    property.PropertyImages.Remove(img);
                    await _unitOfWork.PropertyImages.DeleteAsync(img.Id);
                }

                // Add or update images
                foreach (var imgDto in dto.PropertyImages)
                {
                    if (imgDto.Id.HasValue)
                    {
                        var existingImg = property.PropertyImages.FirstOrDefault(pi => pi.Id == imgDto.Id.Value);
                        if (existingImg != null)
                        {
                            existingImg.ImageUrl = imgDto.ImageUrl;
                            existingImg.Description = imgDto.Description;
                            existingImg.DisplayOrder = imgDto.DisplayOrder;
                            existingImg.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                            await _unitOfWork.PropertyImages.UpdateAsync(existingImg);
                        }
                    }
                    else
                    {
                        var newImg = new PropertyImage
                        {
                            PropertyId = property.Id,
                            ImageUrl = imgDto.ImageUrl,
                            Description = imgDto.Description,
                            DisplayOrder = imgDto.DisplayOrder,
                            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
                        };
                        property.PropertyImages.Add(newImg);
                        await _unitOfWork.PropertyImages.CreateAsync(newImg);
                    }
                }
            }

            await _unitOfWork.Properties.UpdateAsync(property);
            await _unitOfWork.CommitTransactionAsync();

            return MapToResponseDto(property);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<PropertyResponseDto> UpdatePropertyVisitPreferencesAsync(Guid userId, int id, UpdatePropertyVisitPreferencesDto dto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Property not found.");

        if (property.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to update this property.");
        }

        property.VisitPreferences = dto.VisitPreferences;
        property.SpecificVisitDays = dto.SpecificVisitDays;
        property.VisitStartTime = dto.VisitStartTime;
        property.VisitEndTime = dto.VisitEndTime;
        property.UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

        await _unitOfWork.Properties.UpdateAsync(property);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(property);
    }

    /// <inheritdoc />
    public async Task DeletePropertyAsync(Guid userId, int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
        {
            throw new KeyNotFoundException("Property not found.");
        }

        if (property.OwnerId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this property.");
        }

        await _unitOfWork.Properties.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PropertyResponseDto>> GetPropertiesByOwnerIdAsync(Guid ownerId)
    {
        var properties = await _unitOfWork.Properties.GetPropertiesByOwnerIdAsync(ownerId);
        return properties.Select(MapToResponseDto);
    }

    /// <inheritdoc />
    public async Task<PropertyResponseDto> SubmitForVerificationAsync(Guid ownerId, int propertyId)
    {
        var property = await _unitOfWork.Properties.GetByIdWithDocumentsAsync(propertyId)
            ?? throw new KeyNotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You do not have permission to submit this property.");

        if (property.VerificationStatusId != PropertyVerificationStatus.Draft &&
            property.VerificationStatusId != PropertyVerificationStatus.Rejected)
            throw new InvalidOperationException("Only Draft or Rejected properties can be submitted for verification.");

        var hasDeed = property.Documents.Any(d => d.DocumentTypeId == DocumentType.PropertyDeed && d.DeletedAt == null);
        if (!hasDeed)
            throw new InvalidOperationException("A property deed document must be attached before submitting for verification.");

        property.VerificationStatusId = PropertyVerificationStatus.Submitted;
        property.Remarks = null;
        property.VerifiedBy = null;
        await _unitOfWork.Properties.UpdateAsync(property);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(property);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PropertyResponseDto>> GetPendingVerificationAsync()
    {
        var properties = await _unitOfWork.Properties.GetPendingVerificationAsync();
        return properties.Select(MapToResponseDto);
    }

    /// <inheritdoc />
    public async Task<PropertyResponseDto> VerifyPropertyAsync(Guid adminId, int propertyId, bool approve, VerifyRequestDto dto)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(propertyId)
            ?? throw new KeyNotFoundException("Property not found.");

        if (property.VerificationStatusId != PropertyVerificationStatus.Submitted)
            throw new InvalidOperationException("Only Submitted properties can be verified.");

        property.VerificationStatusId = approve
            ? PropertyVerificationStatus.Verified
            : PropertyVerificationStatus.Rejected;

        if (approve)
            property.AvailabilityStatusId = PropertyAvailabilityStatus.Available;

        property.VerifiedBy = adminId;
        property.Remarks = dto.Remarks;

        await _unitOfWork.Properties.UpdateAsync(property);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponseDto(property);
    }

    /// <inheritdoc />
    public async Task<DocumentResponseDto> AddDocumentAsync(Guid ownerId, int propertyId, AddPropertyDocumentDto dto)
    {
        var property = await _unitOfWork.Properties.GetByIdWithDocumentsAsync(propertyId)
            ?? throw new KeyNotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You do not have permission to add documents to this property.");

        var document = new Models.Document
        {
            Id = Guid.NewGuid(),
            DocumentTypeId = dto.DocumentTypeId,
            DocumentNumber = dto.DocumentNumber,
            DocumentUrl = dto.DocumentUrl,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _unitOfWork.Documents.CreateAsync(document);
            await _unitOfWork.Properties.LinkDocumentAsync(property.Id, document.Id);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }

        return MapToDocumentResponseDto(document);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DocumentResponseDto>> GetDocumentsAsync(int propertyId)
    {
        var property = await _unitOfWork.Properties.GetByIdWithDocumentsAsync(propertyId)
            ?? throw new KeyNotFoundException("Property not found.");

        return property.Documents.Select(MapToDocumentResponseDto);
    }

    /// <inheritdoc />
    public async Task RemoveDocumentAsync(Guid ownerId, int propertyId, Guid documentId)
    {
        var property = await _unitOfWork.Properties.GetByIdWithDocumentsAsync(propertyId)
            ?? throw new KeyNotFoundException("Property not found.");

        if (property.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You do not have permission to remove documents from this property.");

        var document = property.Documents.FirstOrDefault(d => d.Id == documentId)
            ?? throw new KeyNotFoundException("Document not found on this property.");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _unitOfWork.Properties.UnlinkDocumentAsync(property.Id, documentId);
            await _unitOfWork.Documents.DeleteAsync(documentId);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static DocumentResponseDto MapToDocumentResponseDto(Models.Document document)
    {
        return new DocumentResponseDto
        {
            Id = document.Id,
            DocumentTypeId = document.DocumentTypeId,
            DocumentNumber = document.DocumentNumber,
            DocumentUrl = document.DocumentUrl
        };
    }

    private static PropertyResponseDto MapToResponseDto(Property property)
    {
        return new PropertyResponseDto
        {
            Id = property.Id,
            OwnerId = property.OwnerId,
            Title = property.Title ?? string.Empty,
            Description = property.Description,
            AddressLine = property.AddressLine ?? string.Empty,
            CityId = property.CityId,
            MonthlyRent = property.MonthlyRent,
            UpfrontPayment = property.UpfrontPayment,
            SecurityDeposit = property.SecurityDeposit,
            ThumbnailImgUrl = property.ThumbnailImgUrl,
            VisitPreferences = property.VisitPreferences,
            SpecificVisitDays = property.SpecificVisitDays,
            VisitStartTime = property.VisitStartTime,
            VisitEndTime = property.VisitEndTime,
            VerificationStatusId = property.VerificationStatusId,
            AvailabilityStatusId = property.AvailabilityStatusId,
            CreatedAt = property.CreatedAt,
            VerifiedBy = property.VerifiedBy,
            Remarks = property.Remarks,
            PropertyImages = property.PropertyImages?.Select(pi => new PropertyImageResponseDto
            {
                Id = pi.Id,
                PropertyId = pi.PropertyId,
                ImageUrl = pi.ImageUrl,
                Description = pi.Description,
                DisplayOrder = pi.DisplayOrder
            }).ToList() ?? new List<PropertyImageResponseDto>(),
            Documents = property.Documents?.Where(d => d.DeletedAt == null)
                .Select(MapToDocumentResponseDto).ToList() ?? new List<DocumentResponseDto>()
        };
    }
}

