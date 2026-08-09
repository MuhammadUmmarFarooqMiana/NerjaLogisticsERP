namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockHistory;

public record StockMovementDto(Guid Id, string Type, int Quantity, DateOnly Date, string? Detail);
