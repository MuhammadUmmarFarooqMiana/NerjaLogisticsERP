using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Reports.Salaries.Queries.GetSalariesReport;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetSalariesReportQuery : IRequest<SalariesReportDto>
{
    public ReportPeriodType PeriodType { get; init; }
    public DateOnly? Date { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public MonthlySummaryStatus? Status { get; init; }
    public Guid? EmployeeId { get; init; }
}
