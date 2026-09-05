using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Expenses.Commands.CreateExpense;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record CreateExpenseCommand : IRequest<Guid>
{
    public ExpenseCategory Category { get; init; }
    public decimal Amount { get; init; }
    public DateOnly ExpenseDate { get; init; }
    public string? Description { get; init; }
    public Guid? PlatformId { get; init; }
}
