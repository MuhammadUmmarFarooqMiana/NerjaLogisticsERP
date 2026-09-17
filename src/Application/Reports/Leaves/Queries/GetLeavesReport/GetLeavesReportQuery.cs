using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Reports.Leaves.Queries.GetLeavesReport;

// Fleet-wide leave reporting: Administrator and Supervisor already review leave requests,
// and Accountant also needs full visibility here — Rider has no use for it.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor},{Roles.Accountant}")]
public record GetLeavesReportQuery : IRequest<LeavesReportDto>
{
    public ReportPeriodType PeriodType { get; init; }
    public DateOnly? Date { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public LeaveStatus? Status { get; init; }
    public Guid? EmployeeId { get; init; }
}
