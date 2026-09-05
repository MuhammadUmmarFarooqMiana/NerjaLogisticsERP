using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Salaries.Commands.UpdateSalaryFormula;

// Platform, FormulaType, and EffectiveFrom are deliberately not editable here — changing any of
// them is really "this is a different formula," which the existing Deactivate + Create lifecycle
// already covers. This command only lets you correct the terms (amount or tier rates) of the
// currently active formula; which of the two fields applies is decided by the stored FormulaType.
[Authorize(Roles = Roles.Administrator)]
public record UpdateSalaryFormulaCommand : IRequest
{
    public Guid Id { get; init; }
    public decimal? FixedMonthlyAmount { get; init; }
    public List<SalaryTierInput> Tiers { get; init; } = new();
}
