using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulas;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Salaries.Queries.GetSalaryFormulaById;

public class GetSalaryFormulaByIdQueryHandler : IRequestHandler<GetSalaryFormulaByIdQuery, SalaryFormulaDto>
{
    private readonly IApplicationDbContext _context;
    public GetSalaryFormulaByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<SalaryFormulaDto> Handle(GetSalaryFormulaByIdQuery request, CancellationToken cancellationToken)
    {
        var formula = await _context.SalaryFormulas
            .Include(f => f.Tiers).Include(f => f.Platform)
            .Where(f => f.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        return formula ?? throw new NotFoundException(nameof(SalaryFormula), request.Id.ToString());
    }
}
