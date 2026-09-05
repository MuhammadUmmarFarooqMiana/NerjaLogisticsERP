using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMyMonthlySummaries;

public class GetMyMonthlySummariesQueryHandler : IRequestHandler<GetMyMonthlySummariesQuery, PaginatedList<MonthlySummaryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyMonthlySummariesQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<PaginatedList<MonthlySummaryDto>> Handle(GetMyMonthlySummariesQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;
        var query = _context.MonthlySummaries.Include(s => s.Employee).Where(s => s.Employee.UserId == userId);

        if (request.Year.HasValue) query = query.Where(s => s.Year == request.Year);
        if (request.Month.HasValue) query = query.Where(s => s.Month == request.Month);

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
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
