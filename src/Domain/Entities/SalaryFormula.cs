namespace NerjaLogisticsERP.Domain.Entities;

public class SalaryFormula : BaseAuditableEntity
{
    private readonly List<SalaryFormulaTier> _tiers = new();

    private SalaryFormula() { }
    private SalaryFormula(Guid? platformId, SalaryFormulaType formulaType, decimal? fixedMonthlyAmount, DateOnly effectiveFrom, Guid createdBy)
    {
        PlatformId = platformId;
        FormulaType = formulaType;
        FixedMonthlyAmount = fixedMonthlyAmount;
        EffectiveFrom = effectiveFrom;
        CreatedByUserId = createdBy;
    }

    public Guid? PlatformId { get; private set; }          // null = generic default, applies when a platform has no formula of its own
    public Platform? Platform { get; private set; }
    public SalaryFormulaType FormulaType { get; private set; }
    public decimal? FixedMonthlyAmount { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }     // null = currently active
    public Guid CreatedByUserId { get; private set; }

    public IReadOnlyCollection<SalaryFormulaTier> Tiers => _tiers.AsReadOnly();

    public static SalaryFormula CreateTiered(Guid? platformId, DateOnly effectiveFrom, Guid createdBy)
        => new(platformId, SalaryFormulaType.TieredPerOrder, null, effectiveFrom, createdBy);

    public static SalaryFormula CreateFixedMonthly(Guid? platformId, decimal amount, DateOnly effectiveFrom, Guid createdBy)
    {
        if (amount <= 0) throw new ArgumentException("Fixed monthly amount must be greater than zero.", nameof(amount));
        return new(platformId, SalaryFormulaType.FixedMonthly, amount, effectiveFrom, createdBy);
    }

    public void AddTier(int minOrders, int? maxOrders, SalaryTierRateType rateType, decimal rate)
    {
        if (FormulaType != SalaryFormulaType.TieredPerOrder)
            throw new InvalidOperationException("Tiers can only be added to a TieredPerOrder formula.");
        if (minOrders < 0) throw new ArgumentException("MinOrders cannot be negative.", nameof(minOrders));
        if (maxOrders.HasValue && maxOrders < minOrders) throw new ArgumentException("MaxOrders cannot be less than MinOrders.", nameof(maxOrders));
        if (rate < 0) throw new ArgumentException("Rate cannot be negative.", nameof(rate));

        _tiers.Add(new SalaryFormulaTier(Id, minOrders, maxOrders, rateType, rate));
    }

    public void Close(DateOnly effectiveTo)
    {
        if (effectiveTo <= EffectiveFrom)
            throw new ArgumentException("EffectiveTo must be greater than EffectiveFrom.", nameof(effectiveTo));

        EffectiveTo = effectiveTo;
    }

    public void UpdateFixedMonthlyAmount(decimal amount)
    {
        if (FormulaType != SalaryFormulaType.FixedMonthly)
            throw new InvalidOperationException("Only FixedMonthly formulas have a fixed monthly amount.");
        if (amount <= 0) throw new ArgumentException("Fixed monthly amount must be greater than zero.", nameof(amount));

        FixedMonthlyAmount = amount;
    }

    /// <summary>Wholesale replace: clears the existing tiers and re-adds the given set, applying AddTier's invariants to each.</summary>
    public void ReplaceTiers(IEnumerable<(int MinOrders, int? MaxOrders, SalaryTierRateType RateType, decimal Rate)> tiers)
    {
        if (FormulaType != SalaryFormulaType.TieredPerOrder)
            throw new InvalidOperationException("Tiers can only be set on a TieredPerOrder formula.");

        _tiers.Clear();
        foreach (var tier in tiers)
            AddTier(tier.MinOrders, tier.MaxOrders, tier.RateType, tier.Rate);
    }
}
