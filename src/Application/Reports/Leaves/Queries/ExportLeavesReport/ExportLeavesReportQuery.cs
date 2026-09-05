using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Reports.Leaves.Queries.ExportLeavesReport;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record ExportLeavesReportQuery : IRequest<DocumentFileResult>
{
    public ReportPeriodType PeriodType { get; init; }
    public ReportFormat Format { get; init; }
    public DateOnly? Date { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public LeaveStatus? Status { get; init; }
    public Guid? EmployeeId { get; init; }
}
