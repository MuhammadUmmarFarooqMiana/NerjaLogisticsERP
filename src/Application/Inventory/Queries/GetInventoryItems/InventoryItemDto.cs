namespace NerjaLogisticsERP.Application.Inventory.Queries.GetInventoryItems;

public record InventoryItemDto(Guid Id, string ItemName, string Unit, int ReorderLevel, int CurrentStock, bool IsLowStock);

