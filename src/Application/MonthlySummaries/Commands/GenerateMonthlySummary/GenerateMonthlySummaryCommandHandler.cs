using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;
using NerjaLogisticsERP.Domain.Services;

namespace NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;

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

        // Closed means the rider ended their day, but a Supervisor/Administrator
        // hasn't approved or rejected it yet — that count isn't settled, so it
        // can't feed into a salary summary. Block generation and let the
        // employee's supervisor know there's something waiting on them, rather
        // than silently excluding those days from the total.
        var pendingApprovalCount = await _context.DailyOrders
            .CountAsync(o => o.EmployeeId == request.EmployeeId && o.OrderDate.Year == request.Year
                && o.OrderDate.Month == request.Month && o.Status == DailyOrderStatus.Closed, cancellationToken);

        if (pendingApprovalCount > 0)
        {
            if (employee.SupervisorId.HasValue)
            {
                var supervisor = await _context.Employees.FindAsync(new object[] { employee.SupervisorId.Value }, cancellationToken);
                if (supervisor is not null)
                {
                    var notice = Notification.Create(
                        supervisor.UserId,
                        "DailyOrdersPendingApproval",
                        "Daily orders awaiting approval",
                        $"{employee.FullName} has {pendingApprovalCount} closed daily order(s) for {request.Year}-{request.Month:D2} " +
                        "still awaiting your approval before their salary summary can be generated.");
                    _context.Notifications.Add(notice);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            throw new ConflictException(
                $"{pendingApprovalCount} daily order(s) for {employee.FullName} in {request.Year}-{request.Month:D2} " +
                "are still awaiting supervisor approval. Approve or reject them before generating a salary summary.");
        }

        // Approved only — a Rejected day contributes 0, matching the supervisor's
        // final call that the claimed count wasn't valid.
        var totalOrders = await _context.DailyOrders
            .Where(o => o.EmployeeId == request.EmployeeId && o.OrderDate.Year == request.Year && o.OrderDate.Month == request.Month && o.Status == DailyOrderStatus.Approved)
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
