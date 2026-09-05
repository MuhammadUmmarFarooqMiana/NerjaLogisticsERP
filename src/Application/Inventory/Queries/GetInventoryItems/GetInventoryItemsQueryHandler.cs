using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Inventory.Queries.GetInventoryItems;

public class GetInventoryItemsQueryHandler : IRequestHandler<GetInventoryItemsQuery, PaginatedList<InventoryItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetInventoryItemsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<InventoryItemDto>> Handle(GetInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.InventoryItems.OrderBy(i => i.ItemName).ToListAsync(cancellationToken);
        var result = items.Select(i => new InventoryItemDto(i.Id, i.ItemName, i.Unit, i.ReorderLevel, i.CurrentStock, i.IsLowStock));
        var filtered = request.LowStockOnly == true ? result.Where(i => i.IsLowStock).ToList() : result.ToList();
        return PaginatedList<InventoryItemDto>.Create(filtered, request.PageNumber, request.PageSize);
    }
}
