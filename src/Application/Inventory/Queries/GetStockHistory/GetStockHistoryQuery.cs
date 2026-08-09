namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockHistory;

public record GetStockHistoryQuery : IRequest<List<StockMovementDto>> { public Guid ItemId { get; init; } }

