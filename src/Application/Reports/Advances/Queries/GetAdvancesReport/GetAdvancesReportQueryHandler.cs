using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Advances.Queries.GetAdvancesReport;

public class GetAdvancesReportQueryHandler : IRequestHandler<GetAdvancesReportQuery, AdvancesReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetAdvancesReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<AdvancesReportDto> Handle(GetAdvancesReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        var query = _context.Advances.Include(a => a.Employee)
            .Where(a => a.AdvanceDate >= period.Start && a.AdvanceDate <= period.End);

        if (request.EmployeeId.HasValue)
            query = query.Where(a => a.EmployeeId == request.EmployeeId);

        var rows = await query
            .OrderBy(a => a.AdvanceDate).ThenBy(a => a.Employee.FullName)
            .Select(a => new AdvancesReportRowDto
            {
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee.FullName,
                Amount = a.Amount,
                Remarks = a.Remarks,
                AdvanceDate = a.AdvanceDate
            })
            .ToListAsync(cancellationToken);

        return new AdvancesReportDto
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
