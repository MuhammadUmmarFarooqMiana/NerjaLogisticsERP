namespace NerjaLogisticsERP.Application.MonthlySummaries;

public record MonthlySummaryDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = default!;
    public int Year { get; init; }
    public int Month { get; init; }
    public int TotalCompletedOrders { get; init; }
    public decimal TotalSalary { get; init; }
    public decimal TotalAdvances { get; init; }
    public decimal TotalFines { get; init; }
    public decimal NetSalaryPayable { get; init; }
    public string Status { get; init; } = default!;
}
