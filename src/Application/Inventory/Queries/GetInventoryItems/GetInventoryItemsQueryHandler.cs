using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.Inventory.Queries.GetInventoryItems;

public class GetInventoryItemsQueryHandler : IRequestHandler<GetInventoryItemsQuery, List<InventoryItemDto>>
{
    private readonly IApplicationDbContext _context;
    public GetInventoryItemsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<InventoryItemDto>> Handle(GetInventoryItemsQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.InventoryItems.OrderBy(i => i.ItemName).ToListAsync(cancellationToken);
        var result = items.Select(i => new InventoryItemDto(i.Id, i.ItemName, i.Unit, i.ReorderLevel, i.CurrentStock, i.IsLowStock));
        return request.LowStockOnly == true ? result.Where(i => i.IsLowStock).ToList() : result.ToList();
    }
}
