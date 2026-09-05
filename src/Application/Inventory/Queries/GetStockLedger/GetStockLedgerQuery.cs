using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockLedger;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant},{Roles.Supervisor}")]
public record GetStockLedgerQuery : IRequest<PaginatedList<StockLedgerEntryDto>>
{
    public Guid? ItemId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
