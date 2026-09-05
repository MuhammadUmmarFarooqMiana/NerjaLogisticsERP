using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockHistory;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant},{Roles.Supervisor}")]
public record GetStockHistoryQuery : IRequest<PaginatedList<StockMovementDto>>
{
    public Guid ItemId { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}

