namespace propertyManagement.DTOs;

/// <summary>
/// A page of results along with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of item contained in the page.</typeparam>
public class PagedResultDto<T>
{
    /// <summary>
    /// Gets or sets the items in the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of items requested per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets the total number of pages.
    /// </summary>
    public int TotalPages { get; set; }
}
