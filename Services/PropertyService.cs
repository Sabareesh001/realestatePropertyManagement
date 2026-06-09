using System;
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

    /// <summary>
    /// Creates/posts a new property. Requires the user to be verified.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner posting the property.</param>
    /// <param name="dto">The property creation details.</param>
    /// <returns>A response DTO detailing the created property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the owner is not verified.</exception>
    public async Task<PropertyResponseDto> CreatePropertyAsync(Guid ownerId, CreatePropertyDto dto)
    {
        var isVerified = await _unitOfWork.UserVerifications.IsUserVerifiedAsync(ownerId);
        if (!isVerified)
        {
            throw new InvalidOperationException("User must be verified to post a property.");
        }

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
            StatusId = 1, // Default status
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Properties.CreateAsync(property);
        await _unitOfWork.SaveChangesAsync();

        return new PropertyResponseDto
        {
            Id = property.Id,
            OwnerId = property.OwnerId,
            Title = property.Title,
            Description = property.Description,
            AddressLine = property.AddressLine ?? string.Empty,
            CityId = property.CityId,
            MonthlyRent = property.MonthlyRent,
            UpfrontPayment = property.UpfrontPayment,
            SecurityDeposit = property.SecurityDeposit,
            ThumbnailImgUrl = property.ThumbnailImgUrl,
            StatusId = property.StatusId,
            CreatedAt = property.CreatedAt,
            VerifiedBy = property.VerifiedBy
        };
    }
}
