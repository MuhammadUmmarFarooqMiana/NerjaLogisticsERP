using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Reports.Orders.Queries.ExportOrdersReport;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record ExportOrdersReportQuery : IRequest<DocumentFileResult>
{
    public ReportPeriodType PeriodType { get; init; }
    public ReportFormat Format { get; init; }
    public DateOnly? Date { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public Guid? PlatformId { get; init; }
    public Guid? EmployeeId { get; init; }
}
