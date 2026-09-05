using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Reports.Expenses.Queries.GetExpensesReport;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetExpensesReportQuery : IRequest<ExpensesReportDto>
{
    public ReportPeriodType PeriodType { get; init; }
    public DateOnly? Date { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public ExpenseCategory? Category { get; init; }
    public Guid? PlatformId { get; init; }
}
