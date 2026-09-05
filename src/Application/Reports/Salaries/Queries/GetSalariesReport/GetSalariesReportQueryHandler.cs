using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Salaries.Queries.GetSalariesReport;

public class GetSalariesReportQueryHandler : IRequestHandler<GetSalariesReportQuery, SalariesReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetSalariesReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SalariesReportDto> Handle(GetSalariesReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        // MonthlySummary is keyed by Year/Month, not a single date — a Year*100+Month
        // comparison against the period's start/end months covers Daily/Weekly/Monthly/Custom
        // selections uniformly (e.g. Daily just narrows to that one month).
        var startYm = period.Start.Year * 100 + period.Start.Month;
        var endYm = period.End.Year * 100 + period.End.Month;

        var query = _context.MonthlySummaries.Include(s => s.Employee)
            .Where(s => (s.Year * 100 + s.Month) >= startYm && (s.Year * 100 + s.Month) <= endYm);

        if (request.Status.HasValue)
            query = query.Where(s => s.Status == request.Status);

        if (request.EmployeeId.HasValue)
            query = query.Where(s => s.EmployeeId == request.EmployeeId);

        var rows = await query
            .OrderByDescending(s => s.Year).ThenByDescending(s => s.Month).ThenBy(s => s.Employee.FullName)
            .Select(s => new SalariesReportRowDto
            {
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FullName,
                PlatformIdNumber = s.Employee.PlatformIdNumber,
                Year = s.Year,
                Month = s.Month,
                TotalCompletedOrders = s.TotalCompletedOrders,
                TotalSalary = s.TotalSalary,
                TotalAdvances = s.TotalAdvances,
                TotalFines = s.TotalFines,
                NetSalaryPayable = s.NetSalaryPayable,
                Status = s.Status.ToString()
            })
            .ToListAsync(cancellationToken);

        return new SalariesReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalSalary = rows.Sum(r => r.TotalSalary),
            TotalAdvances = rows.Sum(r => r.TotalAdvances),
            TotalFines = rows.Sum(r => r.TotalFines),
            TotalNetPayable = rows.Sum(r => r.NetSalaryPayable),
            TotalCount = rows.Count,
            Rows = rows
        };
    }
}
