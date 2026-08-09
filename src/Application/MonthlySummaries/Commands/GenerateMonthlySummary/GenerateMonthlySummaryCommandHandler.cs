using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;
using NerjaLogisticsERP.Domain.Services;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public class GenerateMonthlySummaryCommandHandler : IRequestHandler<GenerateMonthlySummaryCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ISalaryCalculator _calculator;
    private readonly ILogger<GenerateMonthlySummaryCommandHandler> _logger;

    public GenerateMonthlySummaryCommandHandler(IApplicationDbContext context, ISalaryCalculator calculator, ILogger<GenerateMonthlySummaryCommandHandler> logger)
    { _context = context; _calculator = calculator; _logger = logger; }

    public async Task<Guid> Handle(GenerateMonthlySummaryCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var existing = await _context.MonthlySummaries
            .FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId && s.Year == request.Year && s.Month == request.Month, cancellationToken);

        if (existing is not null && existing.Status != MonthlySummaryStatus.Draft)
            throw new ConflictException($"Summary for {request.Year}-{request.Month} is already {existing.Status} and cannot be regenerated.");

        var totalOrders = await _context.DailyOrders
            .Where(o => o.EmployeeId == request.EmployeeId && o.OrderDate.Year == request.Year && o.OrderDate.Month == request.Month && o.Status == DailyOrderStatus.Closed)
            .SumAsync(o => o.CompletedOrders, cancellationToken);

        var asOf = new DateOnly(request.Year, request.Month, 1);
        var formula = await _context.SalaryFormulas.Include(f => f.Tiers)
            .Where(f => f.PlatformId == employee.PlatformId && f.EffectiveFrom <= asOf && (f.EffectiveTo == null || f.EffectiveTo >= asOf))
            .FirstOrDefaultAsync(cancellationToken)
            ?? await _context.SalaryFormulas.Include(f => f.Tiers)
            .Where(f => f.PlatformId == null && f.EffectiveFrom <= asOf && (f.EffectiveTo == null || f.EffectiveTo >= asOf))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("SalaryFormula", "no active formula for platform or generic default");

        var totalSalary = _calculator.Calculate(formula, totalOrders);

        var totalAdvances = await _context.Advances
            .Where(a => a.EmployeeId == request.EmployeeId && a.AdvanceDate.Year == request.Year && a.AdvanceDate.Month == request.Month)
            .SumAsync(a => a.Amount, cancellationToken);

        var totalFines = await _context.Fines
            .Where(f => f.EmployeeId == request.EmployeeId && f.FineDate.Year == request.Year && f.FineDate.Month == request.Month)
            .SumAsync(f => f.Amount, cancellationToken);

        if (existing is not null)
            _context.MonthlySummaries.Remove(existing); // Draft regeneration: replace, don't accumulate duplicates

        var summary = MonthlySummary.Create(request.EmployeeId, request.Year, request.Month, totalOrders, totalSalary, totalAdvances, totalFines);
        _context.MonthlySummaries.Add(summary);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Monthly summary generated: Employee {EmployeeId}, {Year}-{Month}, Net {Net}",
            request.EmployeeId, request.Year, request.Month, summary.NetSalaryPayable);

        return summary.Id;
    }
}
