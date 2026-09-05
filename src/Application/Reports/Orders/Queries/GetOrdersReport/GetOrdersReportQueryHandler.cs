using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Reports.Orders.Queries.GetOrdersReport;

public class GetOrdersReportQueryHandler : IRequestHandler<GetOrdersReportQuery, OrdersReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetOrdersReportQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<OrdersReportDto> Handle(GetOrdersReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        // Only finalized days belong in a report — an in-progress Open day isn't
        // a settled number yet. Closed (awaiting review), Approved, and Rejected
        // all show up so the Status column reflects real review state, mirroring
        // GetDailyOrderHistoryQuery — this is a visibility report, not a payroll
        // input, so a Rejected day still appears (labeled as such) rather than
        // vanishing silently.
        var query = _context.DailyOrders
            .Where(o => o.Status != DailyOrderStatus.Open && o.OrderDate >= period.Start && o.OrderDate <= period.End);

        var isAdministrator = _user.Roles?.Contains(Roles.Administrator) ?? false;
        if (!isAdministrator)
        {
            var userId = _user.Id!.Value;
            var supervisorEmployeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync(cancellationToken);

            query = query.Where(o => o.Employee.SupervisorId == supervisorEmployeeId);
        }

        if (request.PlatformId.HasValue)
            query = query.Where(o => o.Employee.PlatformId == request.PlatformId);

        if (request.EmployeeId.HasValue)
            query = query.Where(o => o.EmployeeId == request.EmployeeId);

        var rows = await query
            .OrderBy(o => o.OrderDate).ThenBy(o => o.Employee.FullName)
            .Select(o => new OrdersReportRowDto
            {
                EmployeeId = o.EmployeeId,
                EmployeeName = o.Employee.FullName,
                PlatformName = o.Employee.Platform != null ? o.Employee.Platform.Name : null,
                OrderDate = o.OrderDate,
                CompletedOrders = o.CompletedOrders,
                Status = o.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        var byPlatform = rows
            .GroupBy(r => r.PlatformName ?? "Unassigned")
            .Select(g => new OrdersReportPlatformBreakdownDto { PlatformName = g.Key, CompletedOrders = g.Sum(r => r.CompletedOrders) })
            .OrderByDescending(g => g.CompletedOrders)
            .ToList();

        return new OrdersReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalCompletedOrders = rows.Sum(r => r.CompletedOrders),
            TotalSessions = rows.Count,
            UniqueRiders = rows.Select(r => r.EmployeeId).Distinct().Count(),
            ByPlatform = byPlatform,
            Rows = rows
        };
    }
}
