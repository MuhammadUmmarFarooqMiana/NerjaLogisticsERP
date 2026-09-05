using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Salaries.Commands.UpdateSalaryFormula;

public class UpdateSalaryFormulaCommandHandler : IRequestHandler<UpdateSalaryFormulaCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateSalaryFormulaCommandHandler> _logger;

    public UpdateSalaryFormulaCommandHandler(IApplicationDbContext context, ILogger<UpdateSalaryFormulaCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UpdateSalaryFormulaCommand request, CancellationToken cancellationToken)
    {
        var formula = await _context.SalaryFormulas
            .Include(f => f.Tiers)
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(SalaryFormula), request.Id.ToString());

        if (formula.EffectiveTo != null)
            throw new ConflictException("This formula has been deactivated and can no longer be edited.");

        if (formula.FormulaType == SalaryFormulaType.FixedMonthly)
        {
            if (request.FixedMonthlyAmount is null)
                throw new NerjaLogisticsERP.Application.Common.Exceptions.ValidationException([new ValidationFailure(nameof(request.FixedMonthlyAmount), "Fixed monthly amount is required.")]);

            formula.UpdateFixedMonthlyAmount(request.FixedMonthlyAmount.Value);
        }
        else
        {
            if (request.Tiers.Count == 0)
                throw new NerjaLogisticsERP.Application.Common.Exceptions.ValidationException([new ValidationFailure(nameof(request.Tiers), "At least one tier is required.")]);

            formula.ReplaceTiers(request.Tiers.Select(t => (t.MinOrders, t.MaxOrders, t.RateType, t.Rate)));

            // Clearing the old tiers out of the navigation is correctly picked up by EF's orphan
            // detection (they get deleted). The newly-added ones are a different story: SalaryFormulaTier
            // gets its Id from a client-generated Guid.NewGuid() in BaseEntity's constructor, so by the
            // time EF's automatic fixup sees them reachable through this already-tracked navigation, a
            // non-default key looks like "existing row" rather than "new" — it tracks them as Modified
            // instead of Added, which produces a no-op UPDATE against a row that was never inserted
            // (surfaced as a DbUpdateConcurrencyException: "expected to affect 1 row(s), but actually
            // affected 0"). Adding them explicitly forces the correct Added state, same as every other
            // "create a child record" handler in this codebase already does via DbSet.Add.
            _context.SalaryFormulaTiers.AddRange(formula.Tiers);
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Salary formula {Id} updated", request.Id);
    }
}
