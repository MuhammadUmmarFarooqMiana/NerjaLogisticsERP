using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Reports.Leaves.Queries.GetLeavesReport;

// Fleet-wide leave reporting mirrors who already reviews leave requests — Administrator
// and Supervisor — Accountant/Rider have no use for it here.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
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
