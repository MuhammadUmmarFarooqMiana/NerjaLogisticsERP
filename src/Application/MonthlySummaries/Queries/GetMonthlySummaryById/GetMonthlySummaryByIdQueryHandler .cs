using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaryById;

public class GetMonthlySummaryByIdQueryHandler : IRequestHandler<GetMonthlySummaryByIdQuery, MonthlySummaryDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetMonthlySummaryByIdQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<MonthlySummaryDetailDto> Handle(GetMonthlySummaryByIdQuery request, CancellationToken cancellationToken)
    {
        var summary = await _context.MonthlySummaries
            .Include(s => s.Employee)
            .Where(s => s.Id == request.Id)
            .Select(s => new MonthlySummaryDetailDto
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
                Status = s.Status.ToString(),
                Created = s.Created,
                VerifiedBy = s.VerifiedBy,
                VerifiedAt = s.VerifiedAt,
                PaidBy = s.PaidBy,
                PaidAt = s.PaidAt,
                PaymentReference = s.PaymentReference
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(MonthlySummary), request.Id.ToString());

        var verifiedByName = summary.VerifiedBy.HasValue
            ? await _identityService.GetUserNameAsync(summary.VerifiedBy.Value)
            : null;
        var paidByName = summary.PaidBy.HasValue
            ? await _identityService.GetUserNameAsync(summary.PaidBy.Value)
            : null;

        return summary with { VerifiedByName = verifiedByName, PaidByName = paidByName };
    }
}
