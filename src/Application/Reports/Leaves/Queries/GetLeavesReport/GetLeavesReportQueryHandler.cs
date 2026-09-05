using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Leaves.Queries.GetLeavesReport;

public class GetLeavesReportQueryHandler : IRequestHandler<GetLeavesReportQuery, LeavesReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetLeavesReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<LeavesReportDto> Handle(GetLeavesReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        var query = _context.LeaveRequests.Include(l => l.Employee)
            .Where(l => l.StartDate >= period.Start && l.StartDate <= period.End);

        if (request.Status.HasValue)
            query = query.Where(l => l.Status == request.Status);

        if (request.EmployeeId.HasValue)
            query = query.Where(l => l.EmployeeId == request.EmployeeId);

        var rows = await query
            .OrderBy(l => l.StartDate).ThenBy(l => l.Employee.FullName)
            .Select(l => new LeavesReportRowDto
            {
                EmployeeId = l.EmployeeId,
                EmployeeName = l.Employee.FullName,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Days = l.EndDate.DayNumber - l.StartDate.DayNumber + 1,
                Reason = l.Reason,
                Status = l.Status.ToString(),
                ReviewedByName = l.ReviewedBy != null
                    ? _context.Employees.Where(e => e.UserId == l.ReviewedBy).Select(e => e.FullName).FirstOrDefault()
                    : null
            })
            .ToListAsync(cancellationToken);

        return new LeavesReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalRequests = rows.Count,
            TotalDays = rows.Sum(r => r.Days),
            ApprovedCount = rows.Count(r => r.Status == "Approved"),
            RejectedCount = rows.Count(r => r.Status == "Rejected"),
            PendingCount = rows.Count(r => r.Status == "Pending"),
            Rows = rows
        };
    }
}
