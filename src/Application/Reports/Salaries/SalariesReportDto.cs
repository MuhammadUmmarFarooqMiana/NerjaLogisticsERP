namespace NerjaLogisticsERP.Application.Reports.Salaries;

public class SalariesReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public decimal TotalSalary { get; set; }
    public decimal TotalAdvances { get; set; }
    public decimal TotalFines { get; set; }
    public decimal TotalNetPayable { get; set; }
    public int TotalCount { get; set; }

    public List<SalariesReportRowDto> Rows { get; set; } = [];
}

public class SalariesReportRowDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string? PlatformIdNumber { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalCompletedOrders { get; set; }
    public decimal TotalSalary { get; set; }
    public decimal TotalAdvances { get; set; }
    public decimal TotalFines { get; set; }
    public decimal NetSalaryPayable { get; set; }
    public string Status { get; set; } = default!;
}
