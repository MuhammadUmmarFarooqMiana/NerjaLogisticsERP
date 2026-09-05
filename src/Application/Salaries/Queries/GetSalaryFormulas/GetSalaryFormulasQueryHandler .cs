using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;

public class GetSalaryFormulasQueryHandler : IRequestHandler<GetSalaryFormulasQuery, PaginatedList<SalaryFormulaDto>>
{
    private readonly IApplicationDbContext _context;
    public GetSalaryFormulasQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<SalaryFormulaDto>> Handle(GetSalaryFormulasQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SalaryFormulas.Include(f => f.Tiers).Include(f => f.Platform).AsQueryable();

        if (request.PlatformId.HasValue)
            query = query.Where(f => f.PlatformId == request.PlatformId);

        if (request.ActiveOnly)
            query = query.Where(f => f.EffectiveTo == null);

        return await query
            .OrderByDescending(f => f.EffectiveFrom)
            .Select(f => new SalaryFormulaDto
            {
                Id = f.Id,
                PlatformId = f.PlatformId,
                PlatformName = f.Platform != null ? f.Platform.Name : "Generic (Default)",
                FormulaType = f.FormulaType.ToString(),
                FixedMonthlyAmount = f.FixedMonthlyAmount,
                EffectiveFrom = f.EffectiveFrom,
                EffectiveTo = f.EffectiveTo,
                Tiers = f.Tiers.Select(t => new SalaryFormulaTierDto
                {
                    MinOrders = t.MinOrders,
                    MaxOrders = t.MaxOrders,
                    RateType = t.RateType.ToString(),
                    Rate = t.Rate
                }).ToList()
            })
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
