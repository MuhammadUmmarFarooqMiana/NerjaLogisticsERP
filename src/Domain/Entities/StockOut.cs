namespace NerjaLogisticsERP.Domain.Entities;

public class StockOut : BaseAuditableEntity
{
    private StockOut() { }
    private StockOut(Guid itemId, Guid employeeId, Guid mechanicId, int quantity, DateOnly stockDate, Guid recordedBy)
    {
        ItemId = itemId;
        EmployeeId = employeeId;
        MechanicId = mechanicId;
        Quantity = quantity;
        StockDate = stockDate;
        RecordedBy = recordedBy;
    }

    public Guid ItemId { get; private set; }
    public InventoryItem Item { get; private set; } = default!;
    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    // Nullable because stock-outs recorded before the Mechanic module existed
    // have none — Create() below still requires one for every new record.
    public Guid? MechanicId { get; private set; }
    public Mechanic? Mechanic { get; private set; }
    public int Quantity { get; private set; }
    public DateOnly StockDate { get; private set; }
    public Guid RecordedBy { get; private set; }

    public static StockOut Create(Guid itemId, Guid employeeId, Guid mechanicId, int quantity, DateOnly stockDate, Guid recordedBy)
    {
        if (itemId == Guid.Empty) throw new ArgumentException("ItemId is required.", nameof(itemId));
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));
        if (mechanicId == Guid.Empty) throw new ArgumentException("MechanicId is required.", nameof(mechanicId));
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        return new StockOut(itemId, employeeId, mechanicId, quantity, stockDate, recordedBy);
    }
}
