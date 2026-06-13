using System;

namespace propertyManagement.Models;

/// <summary>
/// Entity representing an image associated with a property listing.
/// </summary>
public class PropertyImage
{
    /// <summary>
    /// Gets or sets the unique identifier of the property image.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the foreign key referencing the associated property.
    /// </summary>
    public int PropertyId { get; set; }

    /// <summary>
    /// Gets or sets the URL of the image.
    /// </summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// Gets or sets an optional description or caption for the image.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the display order of the image.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

    /// <summary>
    /// Gets or sets the date and time when the record was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the record was soft deleted.
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets the navigation property to the associated property.
    /// </summary>
    public virtual Property Property { get; set; } = null!;
}
