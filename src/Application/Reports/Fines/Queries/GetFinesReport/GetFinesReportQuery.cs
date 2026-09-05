using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Reports.Fines.Queries.GetFinesReport;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public record GetFinesReportQuery : IRequest<FinesReportDto>
{
    public ReportPeriodType PeriodType { get; init; }
    public DateOnly? Date { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public Guid? EmployeeId { get; init; }
}
