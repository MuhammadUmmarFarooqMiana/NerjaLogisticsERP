namespace NerjaLogisticsERP.Application.Reports.Expenses;

public class ExpensesReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public decimal TotalAmount { get; set; }
    public int TotalCount { get; set; }

    public List<ExpensesReportCategoryBreakdownDto> ByCategory { get; set; } = [];
    public List<ExpensesReportRowDto> Rows { get; set; } = [];
}

public class ExpensesReportCategoryBreakdownDto
{
    public string Category { get; set; } = default!;
    public decimal Amount { get; set; }
}

public class ExpensesReportRowDto
{
    public string Category { get; set; } = default!;
    public decimal Amount { get; set; }
    public DateOnly ExpenseDate { get; set; }
    public string? Description { get; set; }
    public string? PlatformName { get; set; }
}
