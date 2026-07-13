namespace propertyManagement.DTOs;

/// <summary>
/// Common query-string pagination parameters accepted by paginated GET list endpoints.
/// </summary>
public class PaginationParams
{
    /// <summary>
    /// Gets or sets the 1-based page number to retrieve. Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page. Defaults to 20.
    /// </summary>
    public int PageSize { get; set; } = 20;
}
