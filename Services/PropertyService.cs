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
    public async Task<PropertyResponseDto> GetPropertyByIdAsync(int id)
    {
        var property = await _unitOfWork.Properties.GetByIdAsync(id);
        if (property == null)
        {
            throw new KeyNotFoundException("Property not found.");
        }
        return MapToResponseDto(property);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync()
    {
        var properties = await _unitOfWork.Properties.GetAllAsync();
        return properties.Select(MapToResponseDto);
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
            }).ToList() ?? new List<PropertyImageResponseDto>()
        };
    }
}

