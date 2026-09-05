namespace NerjaLogisticsERP.Domain.Entities;

public class StockIn : BaseAuditableEntity
{
    private StockIn() { }
    private StockIn(Guid itemId, int quantity, DateOnly stockDate, Guid supplierId, Guid recordedBy)
    {
        ItemId = itemId;
        Quantity = quantity;
        StockDate = stockDate;
        SupplierId = supplierId;
        RecordedBy = recordedBy;
    }

    public Guid ItemId { get; private set; }
    public InventoryItem Item { get; private set; } = default!;
    public int Quantity { get; private set; }
    public DateOnly StockDate { get; private set; }
    public Guid SupplierId { get; private set; }
    public Supplier Supplier { get; private set; } = default!;
    public Guid RecordedBy { get; private set; }

    public static StockIn Create(Guid itemId, int quantity, DateOnly stockDate, Guid supplierId, Guid recordedBy)
    {
        if (itemId == Guid.Empty) throw new ArgumentException("ItemId is required.", nameof(itemId));
        if (supplierId == Guid.Empty) throw new ArgumentException("SupplierId is required.", nameof(supplierId));
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        return new StockIn(itemId, quantity, stockDate, supplierId, recordedBy);
    }
}
