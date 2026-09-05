namespace NerjaLogisticsERP.Application.Salaries.Commands.UpdateSalaryFormula;

public class UpdateSalaryFormulaCommandValidator : AbstractValidator<UpdateSalaryFormulaCommand>
{
    public UpdateSalaryFormulaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        When(x => x.FixedMonthlyAmount.HasValue, () =>
            RuleFor(x => x.FixedMonthlyAmount).GreaterThan(0));

        RuleForEach(x => x.Tiers).ChildRules(tier =>
        {
            tier.RuleFor(t => t.MinOrders).GreaterThanOrEqualTo(0);
            tier.RuleFor(t => t.Rate).GreaterThanOrEqualTo(0);
            tier.RuleFor(t => t.MaxOrders)
                .GreaterThanOrEqualTo(t => t.MinOrders)
                .When(t => t.MaxOrders.HasValue);
        });
    }
}
