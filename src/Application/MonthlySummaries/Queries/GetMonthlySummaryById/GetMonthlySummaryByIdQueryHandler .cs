using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaryById;

public class GetMonthlySummaryByIdQueryHandler : IRequestHandler<GetMonthlySummaryByIdQuery, MonthlySummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetMonthlySummaryByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<MonthlySummaryDto> Handle(GetMonthlySummaryByIdQuery request, CancellationToken cancellationToken)
    {
        var summary = await _context.MonthlySummaries
            .Include(s => s.Employee)
            .Where(s => s.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        return summary ?? throw new NotFoundException(nameof(MonthlySummary), request.Id.ToString());
    }
}
