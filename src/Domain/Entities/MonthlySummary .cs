namespace NerjaLogisticsERP.Domain.Entities;

public class MonthlySummary : BaseAuditableEntity
{
    private MonthlySummary() { }
    private MonthlySummary(Guid employeeId, int year, int month, int totalCompletedOrders,
        decimal totalSalary, decimal totalAdvances, decimal totalFines)
    {
        EmployeeId = employeeId;
        Year = year;
        Month = month;
        TotalCompletedOrders = totalCompletedOrders;
        TotalSalary = totalSalary;
        TotalAdvances = totalAdvances;
        TotalFines = totalFines;
    }

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    public int Year { get; private set; }
    public int Month { get; private set; }
    public int TotalCompletedOrders { get; private set; }
    public decimal TotalSalary { get; private set; }
    public decimal TotalAdvances { get; private set; }
    public decimal TotalFines { get; private set; }
    public decimal NetSalaryPayable => TotalSalary - TotalAdvances - TotalFines;

    public MonthlySummaryStatus Status { get; private set; } = MonthlySummaryStatus.Draft;
    public Guid? VerifiedBy { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public Guid? PaidBy { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public string? PaymentReference { get; private set; }

    public static MonthlySummary Create(Guid employeeId, int year, int month,
        int totalCompletedOrders, decimal totalSalary, decimal totalAdvances, decimal totalFines)
    {
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));
        if (month is < 1 or > 12) throw new ArgumentException("Month must be between 1 and 12.", nameof(month));

        var summary = new MonthlySummary(employeeId, year, month, totalCompletedOrders, totalSalary, totalAdvances, totalFines);
        summary.AddDomainEvent(new MonthlySummaryGeneratedEvent(summary));
        return summary;
    }

    public void Verify(Guid verifiedBy)
    {
        if (Status != MonthlySummaryStatus.Draft)
            throw new InvalidOperationException($"Cannot verify a summary with status {Status}.");

        Status = MonthlySummaryStatus.Verified;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new MonthlySummaryVerifiedEvent(this));
    }

    public void MarkAsPaid(Guid paidBy, string? paymentReference)
    {
        if (Status != MonthlySummaryStatus.Verified)
            throw new InvalidOperationException("A summary must be Verified before it can be marked as Paid.");

        Status = MonthlySummaryStatus.Paid;
        PaidBy = paidBy;
        PaidAt = DateTimeOffset.UtcNow;
        PaymentReference = paymentReference;
        AddDomainEvent(new MonthlySummaryPaidEvent(this));
    }
}
