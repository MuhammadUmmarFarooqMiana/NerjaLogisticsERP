namespace NerjaLogisticsERP.Application.Reports.Inventory;

public class InventoryLedgerReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public int TotalIn { get; set; }
    public int TotalOut { get; set; }
    public int NetChange { get; set; }

    public List<InventoryLedgerReportRowDto> Rows { get; set; } = [];
}

public class InventoryLedgerReportRowDto
{
    public DateOnly Date { get; set; }
    public string ItemName { get; set; } = default!;
    /// <summary>"In" or "Out".</summary>
    public string MovementType { get; set; } = default!;
    public int Quantity { get; set; }
    /// <summary>Supplier name for an In movement, employee name for an Out movement.</summary>
    public string Reference { get; set; } = default!;
    public int RunningBalance { get; set; }
}
