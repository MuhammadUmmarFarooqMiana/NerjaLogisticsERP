namespace NerjaLogisticsERP.Domain.Entities;

public class InventoryItem : BaseAuditableEntity
{
    private InventoryItem() { }
    private InventoryItem(string itemName, string unit, int reorderLevel)
    {
        ItemName = itemName;
        Unit = unit;
        ReorderLevel = reorderLevel;
    }

    public string ItemName { get; private set; } = default!;
    public string Unit { get; private set; } = "pcs";
    public int ReorderLevel { get; private set; }
    public int CurrentStock { get; private set; }

    public static InventoryItem Create(string itemName, string unit, int reorderLevel)
    {
        if (string.IsNullOrWhiteSpace(itemName)) throw new ArgumentException("Item name is required.", nameof(itemName));
        if (reorderLevel < 0) throw new ArgumentException("ReorderLevel cannot be negative.", nameof(reorderLevel));

        return new InventoryItem(itemName, unit, reorderLevel);
    }

    public void ReceiveStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        CurrentStock += quantity;
    }

    public void IssueStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (quantity > CurrentStock)
            throw new InvalidOperationException($"Insufficient stock. Available: {CurrentStock}, requested: {quantity}.");

        CurrentStock -= quantity;
    }

    public bool IsLowStock => CurrentStock <= ReorderLevel;
}
