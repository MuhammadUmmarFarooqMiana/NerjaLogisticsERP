namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockLedger;

public record StockLedgerEntryDto
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
    public string ItemName { get; init; } = default!;
    public string Type { get; init; } = default!;   // "In" or "Out"
    public int Quantity { get; init; }
    public DateOnly Date { get; init; }
    public string? Detail { get; init; }             // Supplier name (In) or Employee name (Out)
}
