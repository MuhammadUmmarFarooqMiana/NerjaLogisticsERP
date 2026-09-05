namespace NerjaLogisticsERP.Domain.Entities;

public class Expense : BaseAuditableEntity
{
    private Expense() { }
    private Expense(ExpenseCategory category, decimal amount, DateOnly expenseDate, string? description, Guid? platformId, Guid recordedBy)
    {
        Category = category;
        Amount = amount;
        ExpenseDate = expenseDate;
        Description = description;
        PlatformId = platformId;
        RecordedBy = recordedBy;
    }

    public ExpenseCategory Category { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly ExpenseDate { get; private set; }
    public string? Description { get; private set; }
    public Guid? PlatformId { get; private set; }
    public Platform? Platform { get; private set; }
    public Guid RecordedBy { get; private set; }

    public static Expense Create(ExpenseCategory category, decimal amount, DateOnly expenseDate, string? description, Guid? platformId, Guid recordedBy)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new Expense(category, amount, expenseDate, description, platformId, recordedBy);
    }
}
