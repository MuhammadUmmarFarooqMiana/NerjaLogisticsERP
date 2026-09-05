using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Inventory.Queries.GetInventoryItems;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant},{Roles.Supervisor}")]
public record GetInventoryItemsQuery : IRequest<PaginatedList<InventoryItemDto>>
{
    public bool? LowStockOnly { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}

