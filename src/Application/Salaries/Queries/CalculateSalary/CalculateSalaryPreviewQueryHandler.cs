using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;
using NerjaLogisticsERP.Domain.Services;

namespace NerjaLogisticsERP.Application.Salaries.Queries.CalculateSalary;

public class CalculateSalaryPreviewQueryHandler : IRequestHandler<CalculateSalaryPreviewQuery, SalaryPreviewDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ISalaryCalculator _calculator;

    public CalculateSalaryPreviewQueryHandler(IApplicationDbContext context, ISalaryCalculator calculator)
    { _context = context; _calculator = calculator; }

    public async Task<SalaryPreviewDto> Handle(CalculateSalaryPreviewQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var totalOrders = await _context.DailyOrders
            .Where(o => o.EmployeeId == request.EmployeeId
                && o.OrderDate.Year == request.Year && o.OrderDate.Month == request.Month
                && o.Status == DailyOrderStatus.Closed)
            .SumAsync(o => o.CompletedOrders, cancellationToken);

        var asOf = new DateOnly(request.Year, request.Month, 1);

        var formula = await _context.SalaryFormulas
            .Include(f => f.Tiers)
            .Where(f => f.PlatformId == employee.PlatformId && f.EffectiveFrom <= asOf && (f.EffectiveTo == null || f.EffectiveTo >= asOf))
            .FirstOrDefaultAsync(cancellationToken)
            ?? await _context.SalaryFormulas
            .Include(f => f.Tiers)
            .Where(f => f.PlatformId == null && f.EffectiveFrom <= asOf && (f.EffectiveTo == null || f.EffectiveTo >= asOf))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("SalaryFormula", "no active formula for platform or generic default");

        var salary = _calculator.Calculate(formula, totalOrders);
        return new SalaryPreviewDto(request.EmployeeId, request.Year, request.Month, totalOrders, salary);
    }
}
