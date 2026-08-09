namespace NerjaLogisticsERP.Domain.Entities;

public class StockOut : BaseAuditableEntity
{
    private StockOut() { }
    private StockOut(Guid itemId, Guid employeeId, int quantity, DateOnly stockDate, Guid recordedBy)
    {
        ItemId = itemId;
        EmployeeId = employeeId;
        Quantity = quantity;
        StockDate = stockDate;
        RecordedBy = recordedBy;
    }

    public Guid ItemId { get; private set; }
    public InventoryItem Item { get; private set; } = default!;
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    public int Quantity { get; private set; }
    public DateOnly StockDate { get; private set; }
    public Guid RecordedBy { get; private set; }

    public static StockOut Create(Guid itemId, Guid employeeId, int quantity, DateOnly stockDate, Guid recordedBy)
    {
        if (itemId == Guid.Empty) throw new ArgumentException("ItemId is required.", nameof(itemId));
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        return new StockOut(itemId, employeeId, quantity, stockDate, recordedBy);
    }
}
