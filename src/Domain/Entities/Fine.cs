namespace NerjaLogisticsERP.Domain.Entities;

public class Fine : BaseAuditableEntity
{
    private Fine() { }
    private Fine(Guid employeeId, decimal amount, string reason, DateOnly fineDate, Guid recordedBy)
    {
        EmployeeId = employeeId;
        Amount = amount;
        Reason = reason;
        FineDate = fineDate;
        RecordedBy = recordedBy;
    }

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public string Reason { get; private set; } = default!;
    public DateOnly FineDate { get; private set; }
    public Guid RecordedBy { get; private set; }

    public static Fine Create(Guid employeeId, decimal amount, string reason, DateOnly fineDate, Guid recordedBy)
    {
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required.", nameof(reason));

        return new Fine(employeeId, amount, reason, fineDate, recordedBy);
    }
}
