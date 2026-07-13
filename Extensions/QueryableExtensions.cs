using Microsoft.EntityFrameworkCore;
using propertyManagement.DTOs;

namespace propertyManagement.Extensions;

/// <summary>
/// Extension methods for paginating EF Core queries and mapping paged results.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Executes an ordered, filtered <see cref="IQueryable{T}"/> as a single page of results.
    /// </summary>
    /// <typeparam name="T">The entity type being queried.</typeparam>
    /// <param name="query">The query to page. Must already include any required filtering and ordering.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A <see cref="PagedResultDto{T}"/> containing the requested page and pagination metadata.</returns>
    public static async Task<PagedResultDto<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    /// <summary>
    /// Pages an already-materialized in-memory sequence. Use only for small, already-loaded
    /// collections (e.g. navigation properties); prefer <see cref="ToPagedResultAsync{T}"/> for database queries.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="source">The in-memory sequence to page.</param>
    /// <param name="pageNumber">The 1-based page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A <see cref="PagedResultDto{T}"/> containing the requested page and pagination metadata.</returns>
    public static PagedResultDto<T> ToPagedResult<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var materialized = source as ICollection<T> ?? source.ToList();
        var totalCount = materialized.Count;
        var items = materialized.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PagedResultDto<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    /// <summary>
    /// Projects the items of a <see cref="PagedResultDto{TEntity}"/> into a <see cref="PagedResultDto{TDto}"/>,
    /// preserving pagination metadata.
    /// </summary>
    /// <typeparam name="TEntity">The source item type.</typeparam>
    /// <typeparam name="TDto">The destination item type.</typeparam>
    /// <param name="source">The paged result to project.</param>
    /// <param name="map">The projection function applied to each item.</param>
    /// <returns>A <see cref="PagedResultDto{TDto}"/> with mapped items and the same pagination metadata.</returns>
    public static PagedResultDto<TDto> Select<TEntity, TDto>(this PagedResultDto<TEntity> source, Func<TEntity, TDto> map)
    {
        return new PagedResultDto<TDto>
        {
            Items = source.Items.Select(map),
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            TotalCount = source.TotalCount,
            TotalPages = source.TotalPages
        };
    }
}
