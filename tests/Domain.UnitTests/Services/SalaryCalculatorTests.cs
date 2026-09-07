using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;
using NerjaLogisticsERP.Domain.Services;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Domain.UnitTests.Services;

public class SalaryCalculatorTests
{
    private static readonly DateOnly EffectiveFrom = new(2026, 1, 1);
    private static readonly Guid CreatedBy = Guid.NewGuid();

    private readonly SalaryCalculator _sut = new();

    private static SalaryFormula FixedMonthly(decimal amount)
        => SalaryFormula.CreateFixedMonthly(platformId: null, amount, EffectiveFrom, CreatedBy);

    private static SalaryFormula Tiered(params (int MinOrders, int? MaxOrders, SalaryTierRateType RateType, decimal Rate)[] tiers)
    {
        var formula = SalaryFormula.CreateTiered(platformId: null, EffectiveFrom, CreatedBy);
        foreach (var tier in tiers)
            formula.AddTier(tier.MinOrders, tier.MaxOrders, tier.RateType, tier.Rate);
        return formula;
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(500)]
    public void Calculate_FixedMonthly_ReturnsConfiguredAmount_RegardlessOfOrders(int completedOrders)
    {
        var formula = FixedMonthly(2500m);

        _sut.Calculate(formula, completedOrders).ShouldBe(2500m);
    }

    // Graduated/marginal brackets, no spike tier: 1-50 @ 10/order, 51-100 @ 15/order, 101+ @ 20/order —
    // each bracket only charges its own rate for the orders that actually fall inside it.
    [TestCase(0, 0)]
    [TestCase(30, 300)]     // 30 orders entirely inside the first bracket: 30 * 10
    [TestCase(50, 500)]     // exactly the first bracket's upper boundary: 50 * 10
    [TestCase(51, 515)]     // one order into the second bracket: (50 * 10) + (1 * 15)
    [TestCase(75, 875)]     // spanning two brackets: (50 * 10) + (25 * 15)
    [TestCase(120, 1650)]   // spanning all three, including the open-ended top bracket:
                             // (50 * 10) + (50 * 15) + (20 * 20)
    public void Calculate_TieredNoSpike_SumsMarginalRatePerBracket(int completedOrders, decimal expected)
    {
        var formula = Tiered(
            (1, 50, SalaryTierRateType.PerOrder, 10m),
            (51, 100, SalaryTierRateType.PerOrder, 15m),
            (101, null, SalaryTierRateType.PerOrder, 20m));

        _sut.Calculate(formula, completedOrders).ShouldBe(expected);
    }

    // A "spike" (FixedTotal) tier models a formula like Hunger's: a flat bonus for hitting a target
    // order count, not a per-order rate. Below the spike's MinOrders, only PerOrder tiers whose
    // MaxOrders is below the spike threshold count at all — CalculateBracketSum is never told about
    // the spike or anything above it.
    [TestCase(50, 250)]   // 50 * 5
    [TestCase(99, 495)]   // 99 * 5, one order short of the spike threshold
    public void Calculate_TieredWithSpike_BelowSpikeThreshold_UsesOnlyBelowSpikePerOrderTiers(int completedOrders, decimal expected)
    {
        var formula = Tiered(
            (1, 99, SalaryTierRateType.PerOrder, 5m),
            (100, 100, SalaryTierRateType.FixedTotal, 2500m),
            (101, null, SalaryTierRateType.PerOrder, 30m));

        _sut.Calculate(formula, completedOrders).ShouldBe(expected);
    }

    // Landing on exactly the spike's MinOrders pays the flat spike Rate ONLY — none of the
    // below-spike per-order accumulation applies. This is the exact shape of the "100 orders -> a
    // single flat amount" question raised earlier about the Hunger formula: with these numbers,
    // 100 orders pays exactly 2500, not 99 orders' worth of per-order pay plus anything extra.
    [Test]
    public void Calculate_TieredWithSpike_ExactlyAtSpikeThreshold_ReturnsSpikeRateOnly()
    {
        var formula = Tiered(
            (1, 99, SalaryTierRateType.PerOrder, 5m),
            (100, 100, SalaryTierRateType.FixedTotal, 2500m),
            (101, null, SalaryTierRateType.PerOrder, 30m));

        _sut.Calculate(formula, completedOrders: 100).ShouldBe(2500m);
    }

    // Beyond the spike: flat spike Rate, plus the "above" tier's per-order rate for every order past
    // the spike threshold (not a further bracket sum — see the FirstOrDefault-ordering test below for
    // what happens when more than one qualifying "above" tier exists).
    [TestCase(101, 2530)]  // 2500 + (1 * 30)
    [TestCase(150, 4000)]  // 2500 + (50 * 30)
    public void Calculate_TieredWithSpike_AboveSpikeThreshold_AddsPerOrderRateForExtraOrders(int completedOrders, decimal expected)
    {
        var formula = Tiered(
            (1, 99, SalaryTierRateType.PerOrder, 5m),
            (100, 100, SalaryTierRateType.FixedTotal, 2500m),
            (101, null, SalaryTierRateType.PerOrder, 30m));

        _sut.Calculate(formula, completedOrders).ShouldBe(expected);
    }

    [Test]
    public void Calculate_TieredWithSpike_AboveSpikeThreshold_NoAboveTierConfigured_ReturnsSpikeRateOnly()
    {
        var formula = Tiered(
            (1, 99, SalaryTierRateType.PerOrder, 5m),
            (100, 100, SalaryTierRateType.FixedTotal, 2500m));
            // no PerOrder tier above the spike at all

        _sut.Calculate(formula, completedOrders: 200).ShouldBe(2500m);
    }

    // Fixed: the above-spike tier is now picked by whichever tier's own [MinOrders, MaxOrders]
    // range actually contains completedOrders — not by insertion order. Tiers are deliberately
    // added out of ascending order here (the higher-MinOrders one first) specifically to prove
    // insertion order no longer affects the result.
    [TestCase(150, 2500 + 30 * 50)]   // 150 falls inside 101-200 (rate 30): extraOrders = 150-100 = 50
    [TestCase(250, 2500 + 50 * 150)]  // 250 falls inside 201+ (rate 50): extraOrders = 250-100 = 150
    public void Calculate_TieredWithSpike_MultipleAboveTiers_PicksTheTierWhoseRangeContainsTheOrderCount(int completedOrders, decimal expected)
    {
        var formula = Tiered(
            (1, 99, SalaryTierRateType.PerOrder, 5m),
            (100, 100, SalaryTierRateType.FixedTotal, 2500m),
            (201, null, SalaryTierRateType.PerOrder, 50m),   // added first, MinOrders further away
            (101, 200, SalaryTierRateType.PerOrder, 30m));   // added second, but the nearer tier for 150

        _sut.Calculate(formula, completedOrders).ShouldBe(expected);
    }

    // A gap between the configured above-spike tiers (151-200 isn't covered by either) falls back
    // to 0 extra per-order pay rather than guessing — same safe fallback as "no above-tier at all".
    [Test]
    public void Calculate_TieredWithSpike_OrderCountFallsInAGapBetweenAboveTiers_AddsNoExtraPerOrderPay()
    {
        var formula = Tiered(
            (1, 99, SalaryTierRateType.PerOrder, 5m),
            (100, 100, SalaryTierRateType.FixedTotal, 2500m),
            (101, 150, SalaryTierRateType.PerOrder, 30m),
            (201, null, SalaryTierRateType.PerOrder, 50m));
            // nothing covers 151-200

        _sut.Calculate(formula, completedOrders: 175).ShouldBe(2500m);
    }
}
