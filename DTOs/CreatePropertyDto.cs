namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for creating a new property.
/// </summary>
public class CreatePropertyDto
{
    /// <summary>
    /// Gets or sets the title of the property.
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the detailed description of the property.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the address line.
    /// </summary>
    public string AddressLine { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the city.
    /// </summary>
    public int CityId { get; set; }

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
    /// Gets or sets the URL of the thumbnail image.
    /// </summary>
    public string? ThumbnailImgUrl { get; set; }
}
