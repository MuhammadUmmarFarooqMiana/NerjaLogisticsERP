using Microsoft.EntityFrameworkCore;

namespace NerjaLogisticsERP.Application.Common.Models;

/// <summary>
/// Standard Clean Architecture template pagination envelope. Query handlers that support
/// paging return this instead of a bare List&lt;T&gt; — the controller unwraps <see cref="Items"/>
/// into the response body (keeping the existing JSON array contract for every caller that
/// doesn't ask for paging) and surfaces the rest of this metadata via an X-Pagination header.
/// </summary>
public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling(count / (double)pageSize) : count > 0 ? 1 : 0;
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    /// <summary>Wraps an already-materialized, unpaged list — used when the caller didn't ask for a page.</summary>
    public static PaginatedList<T> Create(List<T> items) =>
        new(items, items.Count, 1, Math.Max(items.Count, 1));

    /// <summary>
    /// In-memory counterpart to <see cref="ToPaginatedListAsync"/> — for handlers whose final shape
    /// only exists after C# post-processing (a GroupJoin materialized then re-projected, two query
    /// results concatenated, etc.) that can't be pushed down into a single Skip/Take SQL query.
    /// </summary>
    public static PaginatedList<T> Create(List<T> items, int? pageNumber, int? pageSize)
    {
        if (pageNumber is > 0 && pageSize is > 0)
        {
            var page = items.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList();
            return new PaginatedList<T>(page, items.Count, pageNumber.Value, pageSize.Value);
        }

        return Create(items);
    }
}
