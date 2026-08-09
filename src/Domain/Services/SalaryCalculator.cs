namespace NerjaLogisticsERP.Domain.Services;

public class SalaryCalculator : ISalaryCalculator
{
    public decimal Calculate(SalaryFormula formula, int completedOrders)
    {
        if (formula.FormulaType == SalaryFormulaType.FixedMonthly)
            return formula.FixedMonthlyAmount ?? 0m;

        var spike = formula.Tiers.FirstOrDefault(t => t.RateType == SalaryTierRateType.FixedTotal);

        if (spike is null)
            return CalculateBracketSum(formula.Tiers, completedOrders);

        if (completedOrders < spike.MinOrders)
            return CalculateBracketSum(formula.Tiers.Where(t => t.RateType == SalaryTierRateType.PerOrder && t.MaxOrders < spike.MinOrders), completedOrders);

        if (completedOrders == spike.MinOrders)
            return spike.Rate;

        var aboveTier = formula.Tiers.FirstOrDefault(t => t.RateType == SalaryTierRateType.PerOrder && t.MinOrders > spike.MinOrders);
        var extraOrders = completedOrders - spike.MinOrders;

        return spike.Rate + (aboveTier?.Rate ?? 0m) * extraOrders;
    }

    private static decimal CalculateBracketSum(IEnumerable<SalaryFormulaTier> tiers, int completedOrders)
    {
        decimal total = 0;
        foreach (var tier in tiers.Where(t => t.RateType == SalaryTierRateType.PerOrder).OrderBy(t => t.MinOrders))
        {
            if (completedOrders < tier.MinOrders) continue;
            var upper = tier.MaxOrders.HasValue ? Math.Min(tier.MaxOrders.Value, completedOrders) : completedOrders;
            var ordersInTier = upper - tier.MinOrders + 1;
            if (ordersInTier > 0) total += ordersInTier * tier.Rate;
        }
        return total;
    }
}
