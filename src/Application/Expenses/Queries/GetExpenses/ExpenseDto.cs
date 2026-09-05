namespace NerjaLogisticsERP.Application.Expenses.Queries.GetExpenses;

public record ExpenseDto
{
    public Guid Id { get; init; }
    public string Category { get; init; } = default!;
    public decimal Amount { get; init; }
    public DateOnly ExpenseDate { get; init; }
    public string? Description { get; init; }
    public Guid? PlatformId { get; init; }
    public string? PlatformName { get; init; }
}
