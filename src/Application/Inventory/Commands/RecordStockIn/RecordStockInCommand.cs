namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockIn;

public record RecordStockInCommand : IRequest<Guid>
{
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public DateOnly StockDate { get; init; }
    public Guid SupplierId { get; init; }
}
