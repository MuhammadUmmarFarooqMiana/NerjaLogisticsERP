namespace NerjaLogisticsERP.Application.Inventory.Commands.RecordStockOut;


public record RecordStockOutCommand : IRequest<Guid>
{
    public Guid ItemId { get; init; }
    public Guid EmployeeId { get; init; }
    public int Quantity { get; init; }
    public DateOnly StockDate { get; init; }
}
