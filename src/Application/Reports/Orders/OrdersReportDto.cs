namespace NerjaLogisticsERP.Application.Reports.Orders;

public class OrdersReportDto
{
    public string PeriodType { get; set; } = default!;
    public string PeriodLabel { get; set; } = default!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }

    public int TotalCompletedOrders { get; set; }
    public int TotalSessions { get; set; }
    public int UniqueRiders { get; set; }

    public List<OrdersReportPlatformBreakdownDto> ByPlatform { get; set; } = [];
    public List<OrdersReportRowDto> Rows { get; set; } = [];
}

public class OrdersReportPlatformBreakdownDto
{
    public string PlatformName { get; set; } = default!;
    public int CompletedOrders { get; set; }
}

public class OrdersReportRowDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = default!;
    public string? PlatformName { get; set; }
    public DateOnly OrderDate { get; set; }
    public int CompletedOrders { get; set; }
    public string Status { get; set; } = default!;
}
