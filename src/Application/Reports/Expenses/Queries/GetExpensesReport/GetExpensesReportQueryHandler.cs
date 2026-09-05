using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Reports.Common;

namespace NerjaLogisticsERP.Application.Reports.Expenses.Queries.GetExpensesReport;

public class GetExpensesReportQueryHandler : IRequestHandler<GetExpensesReportQuery, ExpensesReportDto>
{
    private readonly IApplicationDbContext _context;
    public GetExpensesReportQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<ExpensesReportDto> Handle(GetExpensesReportQuery request, CancellationToken cancellationToken)
    {
        var period = ReportPeriodResolver.Resolve(
            request.PeriodType, request.Date, request.Year, request.Month, request.StartDate, request.EndDate);

        var query = _context.Expenses.Include(e => e.Platform)
            .Where(e => e.ExpenseDate >= period.Start && e.ExpenseDate <= period.End);

        if (request.Category.HasValue)
            query = query.Where(e => e.Category == request.Category);

        if (request.PlatformId.HasValue)
            query = query.Where(e => e.PlatformId == request.PlatformId);

        var rows = await query
            .OrderBy(e => e.ExpenseDate)
            .Select(e => new ExpensesReportRowDto
            {
                Category = e.Category.ToString(),
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Description = e.Description,
                PlatformName = e.Platform != null ? e.Platform.Name : null
            })
            .ToListAsync(cancellationToken);

        var byCategory = rows
            .GroupBy(r => r.Category)
            .Select(g => new ExpensesReportCategoryBreakdownDto { Category = g.Key, Amount = g.Sum(r => r.Amount) })
            .OrderByDescending(g => g.Amount)
            .ToList();

        return new ExpensesReportDto
        {
            PeriodType = request.PeriodType.ToString(),
            PeriodLabel = period.Label,
            PeriodStart = period.Start,
            PeriodEnd = period.End,
            TotalAmount = rows.Sum(r => r.Amount),
            TotalCount = rows.Count,
            ByCategory = byCategory,
            Rows = rows
        };
    }
}
