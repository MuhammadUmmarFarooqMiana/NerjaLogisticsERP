using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Expenses.Queries.GetExpenses;

public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, PaginatedList<ExpenseDto>>
{
    private readonly IApplicationDbContext _context;
    public GetExpensesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<ExpenseDto>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Expenses.Include(e => e.Platform).AsQueryable();

        if (request.Category.HasValue)
            query = query.Where(e => e.Category == request.Category);

        if (request.PlatformId.HasValue)
            query = query.Where(e => e.PlatformId == request.PlatformId);

        if (request.StartDate.HasValue)
            query = query.Where(e => e.ExpenseDate >= request.StartDate);

        if (request.EndDate.HasValue)
            query = query.Where(e => e.ExpenseDate <= request.EndDate);

        return await query
            .OrderByDescending(e => e.ExpenseDate)
            .Select(e => new ExpenseDto
            {
                Id = e.Id,
                Category = e.Category.ToString(),
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Description = e.Description,
                PlatformId = e.PlatformId,
                PlatformName = e.Platform != null ? e.Platform.Name : null
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
