namespace propertyManagement.DTOs;

/// <summary>
/// Data Transfer Object for adding an image during property creation.
/// </summary>
public class CreatePropertyImageDto
{
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
