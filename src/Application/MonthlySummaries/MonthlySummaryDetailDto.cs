namespace NerjaLogisticsERP.Application.MonthlySummaries;

public record MonthlySummaryDetailDto
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
    public DateTimeOffset Created { get; init; }

    public Guid? VerifiedBy { get; init; }

    // Sourced from ApplicationUser (Identity), not the MonthlySummary table itself —
    // populated via IIdentityService since Application can't reference the
    // Infrastructure-layer ApplicationUser type directly.
    public string? VerifiedByName { get; init; }
    public DateTimeOffset? VerifiedAt { get; init; }

    public Guid? PaidBy { get; init; }
    public string? PaidByName { get; init; }
    public DateTimeOffset? PaidAt { get; init; }
    public string? PaymentReference { get; init; }
}
