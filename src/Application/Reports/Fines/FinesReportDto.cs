namespace NerjaLogisticsERP.Application.Reports.Fines;

public class FinesReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public decimal TotalAmount { get; set; }
    public int TotalCount { get; set; }

    public List<FinesReportRowDto> Rows { get; set; } = [];
}

public class FinesReportRowDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = default!;
    public DateOnly FineDate { get; set; }
}
