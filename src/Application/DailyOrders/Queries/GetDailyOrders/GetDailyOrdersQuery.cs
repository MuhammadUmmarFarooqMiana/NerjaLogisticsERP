using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.DailyOrders.Queries;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.DailyOrders.Queries.GetDailyOrders;

// Team view: Administrator sees every rider, Supervisor sees only their own
// team (scoped by Employee.SupervisorId in the handler). Defaults to today.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record GetDailyOrdersQuery : IRequest<PaginatedList<DailyOrderListItemDto>>
{
    public DateOnly? Date { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
