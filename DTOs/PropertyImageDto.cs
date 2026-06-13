using System;

namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for managing an image during property updates.
/// </summary>
public class PropertyImageDto
{
    /// <summary>
    /// Gets or sets the identifier of the image. If null, it indicates a new image is being added.
    /// </summary>
    public Guid? Id { get; set; }

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
