namespace NerjaLogisticsERP.Domain.Entities;

public class Advance : BaseAuditableEntity
{
    private Advance() { }
    private Advance(Guid employeeId, decimal amount, DateOnly advanceDate, string? remarks, Guid recordedBy)
    {
        EmployeeId = employeeId;
        Amount = amount;
        AdvanceDate = advanceDate;
        Remarks = remarks;
        RecordedBy = recordedBy;
    }

    public Guid EmployeeId { get; private set; }
    public Employee Employee { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public DateOnly AdvanceDate { get; private set; }
    public string? Remarks { get; private set; }
    public Guid RecordedBy { get; private set; }

    public static Advance Create(Guid employeeId, decimal amount, DateOnly advanceDate, string? remarks, Guid recordedBy)
    {
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new Advance(employeeId, amount, advanceDate, remarks, recordedBy);
    }
}
