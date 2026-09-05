using Microsoft.EntityFrameworkCore;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Common.Mappings;

public static class MappingExtensions
{
    /// <summary>
    /// Applies server-side paging when both <paramref name="pageNumber"/> and <paramref name="pageSize"/>
    /// are supplied (a real Skip/Take query against the database); otherwise materializes the whole
    /// query, exactly matching every handler's pre-pagination behavior. Callers that never pass paging
    /// params — every existing frontend call today — get identical results to before this existed.
    /// </summary>
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> queryable,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber is > 0 && pageSize is > 0)
        {
            return await PaginatedList<T>.CreateAsync(queryable, pageNumber.Value, pageSize.Value, cancellationToken);
        }

        var items = await queryable.ToListAsync(cancellationToken);
        return PaginatedList<T>.Create(items);
    }
}
