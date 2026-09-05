using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Reports.Orders.Queries.GetOrdersReport;

// Fleet-wide reporting: Administrator sees every rider, Supervisor is scoped to
// their own team (mirrors GetDailyOrderHistoryQuery) — Accountant/Rider have no
// use for a fleet orders report, so unlike the self-service queries this is
// role-gated rather than open to every authenticated user.
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record GetOrdersReportQuery : IRequest<OrdersReportDto>
{
    public ReportPeriodType PeriodType { get; init; }

    /// <summary>Daily/Weekly anchor date. Defaults to today when omitted.</summary>
    public DateOnly? Date { get; init; }

    /// <summary>Monthly period. Both default to the current year/month when omitted.</summary>
    public int? Year { get; init; }
    public int? Month { get; init; }

    /// <summary>Custom period — both required when PeriodType is Custom.</summary>
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }

    public Guid? PlatformId { get; init; }
    public Guid? EmployeeId { get; init; }
}
