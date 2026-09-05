namespace NerjaLogisticsERP.Application.Reports.Advances;

public class AdvancesReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public decimal TotalAmount { get; set; }
    public int TotalCount { get; set; }

    public List<AdvancesReportRowDto> Rows { get; set; } = [];
}

public class AdvancesReportRowDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string? Remarks { get; set; }
    public DateOnly AdvanceDate { get; set; }
}
