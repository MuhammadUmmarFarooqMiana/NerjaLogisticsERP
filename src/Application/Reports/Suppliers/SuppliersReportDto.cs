namespace NerjaLogisticsERP.Application.Reports.Suppliers;

public class SuppliersReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public int TotalSuppliers { get; set; }
    public int TotalTransactions { get; set; }
    public int TotalQuantityReceived { get; set; }

    public List<SuppliersReportRowDto> Rows { get; set; } = [];
}

public class SuppliersReportRowDto
{
    public string SupplierName { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }
    public DateOnly StockDate { get; set; }
}
