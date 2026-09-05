using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Fines.Queries.GetFinesReport;

public class GetFinesReportQueryHandler : IRequestHandler<GetFinesReportQuery, FinesReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetFinesReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<FinesReportDto> Handle(GetFinesReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        var query = _context.Fines.Include(f => f.Employee)
            .Where(f => f.FineDate >= period.Start && f.FineDate <= period.End);

        if (request.EmployeeId.HasValue)
            query = query.Where(f => f.EmployeeId == request.EmployeeId);

        var rows = await query
            .OrderBy(f => f.FineDate).ThenBy(f => f.Employee.FullName)
            .Select(f => new FinesReportRowDto
            {
                EmployeeId = f.EmployeeId,
                EmployeeName = f.Employee.FullName,
                Amount = f.Amount,
                Reason = f.Reason,
                FineDate = f.FineDate
            })
            .ToListAsync(cancellationToken);

        return new FinesReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalAmount = rows.Sum(r => r.Amount),
            TotalCount = rows.Count,
            Rows = rows
        };
    }
}
