using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object representing a property image in API responses.
/// </summary>
public class PropertyImageResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the property image.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated property.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// Gets or sets the image URL.
    /// </summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description of the image.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the display order of the image.
    /// </summary>
    public int DisplayOrder { get; set; }
}
