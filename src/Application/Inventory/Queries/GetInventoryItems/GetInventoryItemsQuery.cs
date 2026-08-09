namespace NerjaLogisticsERP.Application.Inventory.Queries.GetInventoryItems;

public record GetInventoryItemsQuery : IRequest<List<InventoryItemDto>> { public bool? LowStockOnly { get; init; } }

