using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.DailyOrders.Queries;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetDailyOrderHistory;

// Completed Order History: Administrator sees every employee, Supervisor is
// scoped to their own team, and a plain Rider is scoped to just themselves —
// the handler decides which of these applies, so every authenticated role
// can call this.
[Authorize]
public record GetDailyOrderHistoryQuery : IRequest<PaginatedList<DailyOrderListItemDto>>
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    /// <summary>Both null (the default) returns every row — the Dashboard's charting/aggregation
    /// calls rely on this and never pass paging params, so their behavior is unaffected.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
