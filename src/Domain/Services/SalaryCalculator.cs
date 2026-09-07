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

        // Pick the above-spike tier whose own [MinOrders, MaxOrders] range actually contains
        // completedOrders — not just the first PerOrder tier above the spike in list order,
        // which picked the wrong rate whenever more than one such tier existed and they weren't
        // authored in ascending order. No matching tier (a gap in the configured ranges) falls
        // back to 0 extra per-order pay, same as before.
        var aboveTier = formula.Tiers.FirstOrDefault(t =>
            t.RateType == SalaryTierRateType.PerOrder
            && t.MinOrders > spike.MinOrders
            && completedOrders >= t.MinOrders
            && (t.MaxOrders is null || completedOrders <= t.MaxOrders));
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
