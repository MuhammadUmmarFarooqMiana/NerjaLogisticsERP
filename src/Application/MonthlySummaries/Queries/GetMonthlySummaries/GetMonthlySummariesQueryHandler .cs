using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaries;

public class GetMonthlySummariesQueryHandler : IRequestHandler<GetMonthlySummariesQuery, List<MonthlySummaryDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMonthlySummariesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<MonthlySummaryDto>> Handle(GetMonthlySummariesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.MonthlySummaries.Include(s => s.Employee).AsQueryable();

        if (request.EmployeeId.HasValue) query = query.Where(s => s.EmployeeId == request.EmployeeId);
        if (request.Year.HasValue) query = query.Where(s => s.Year == request.Year);
        if (request.Month.HasValue) query = query.Where(s => s.Month == request.Month);
        if (request.Status.HasValue) query = query.Where(s => s.Status == request.Status);

        return await query
            .OrderByDescending(s => s.Year).ThenByDescending(s => s.Month)
            .Select(s => new MonthlySummaryDto
            {
                Id = s.Id,
                EmployeeId = s.EmployeeId,
                EmployeeName = s.Employee.FullName,
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
    }
}
