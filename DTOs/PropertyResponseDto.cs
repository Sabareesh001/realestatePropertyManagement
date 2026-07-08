using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a property in API responses.
/// </summary>
public class PropertyResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the property.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the property owner.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the title of the property.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the property.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the address line of the property.
    /// </summary>
    public string AddressLine { get; set; } = null!;

    /// <summary>
    /// Gets or sets the city identifier.
    /// </summary>
    public int? CityId { get; set; }

    /// <summary>
    /// Gets or sets the monthly rent amount.
    /// </summary>
    public decimal MonthlyRent { get; set; }

    /// <summary>
    /// Gets or sets the upfront payment amount.
    /// </summary>
    public decimal UpfrontPayment { get; set; }

    /// <summary>
    /// Gets or sets the security deposit amount.
    /// </summary>
    public decimal SecurityDeposit { get; set; }

    /// <summary>
    /// Gets or sets the thumbnail image URL.
    /// </summary>
    public string? ThumbnailImgUrl { get; set; }

    /// <summary>
    /// Gets or sets the verification status identifier of the property.
    /// </summary>
    public int? VerificationStatusId { get; set; }

    /// <summary>
    /// Gets or sets the availability status identifier of the property.
    /// </summary>
    public int? AvailabilityStatusId { get; set; }

    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the admin who verified the property.
    /// </summary>
    public Guid? VerifiedBy { get; set; }

    /// <summary>
    /// Gets or sets verification or rejection remarks for the property.
    /// </summary>
    public string? Remarks { get; set; }

    /// <summary>
    /// Gets or sets the list of images associated with the property.
    /// </summary>
    public System.Collections.Generic.ICollection<PropertyImageResponseDto> PropertyImages { get; set; } = new System.Collections.Generic.List<PropertyImageResponseDto>();

    /// <summary>
    /// Gets or sets the list of documents associated with the property.
    /// </summary>
    public System.Collections.Generic.ICollection<DocumentResponseDto> Documents { get; set; } = new System.Collections.Generic.List<DocumentResponseDto>();

    public string? VisitPreferences { get; set; }
    public string? SpecificVisitDays { get; set; }
    public System.TimeSpan? VisitStartTime { get; set; }
    public System.TimeSpan? VisitEndTime { get; set; }
}
