using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;

public class CreateSalaryFormulaCommandValidator : AbstractValidator<CreateSalaryFormulaCommand>
{
    public CreateSalaryFormulaCommandValidator()
    {
        RuleFor(x => x.EffectiveFrom).NotEmpty();
        When(x => x.FormulaType == SalaryFormulaType.FixedMonthly, () =>
            RuleFor(x => x.FixedMonthlyAmount).NotNull().GreaterThan(0));
        When(x => x.FormulaType == SalaryFormulaType.TieredPerOrder, () =>
            RuleFor(x => x.Tiers).NotEmpty());
    }
}
