using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Expenses.Queries.GetExpenses;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetExpensesQuery : IRequest<PaginatedList<ExpenseDto>>
{
    public ExpenseCategory? Category { get; init; }
    public Guid? PlatformId { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
