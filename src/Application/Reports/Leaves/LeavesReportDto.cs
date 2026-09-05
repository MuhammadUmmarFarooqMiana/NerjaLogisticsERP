namespace NerjaLogisticsERP.Application.Reports.Leaves;

public class LeavesReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public int TotalRequests { get; set; }
    public int TotalDays { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int PendingCount { get; set; }

    public List<LeavesReportRowDto> Rows { get; set; } = [];
}

public class LeavesReportRowDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int Days { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = default!;
    public string? ReviewedByName { get; set; }
}
