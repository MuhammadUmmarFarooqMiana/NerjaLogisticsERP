namespace NerjaLogisticsERP.Application.Inventory.Queries.GetStockLedger;

public record GetStockLedgerQuery : IRequest<List<StockLedgerEntryDto>>
{
    public Guid? ItemId { get; init; }
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
}
