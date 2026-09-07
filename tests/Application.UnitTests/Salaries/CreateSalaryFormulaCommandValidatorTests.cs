using NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Salaries;

public class CreateSalaryFormulaCommandValidatorTests
{
    private readonly CreateSalaryFormulaCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenEffectiveFromIsDefault()
        => _validator.Validate(new CreateSalaryFormulaCommand
            {
                FormulaType = SalaryFormulaType.FixedMonthly,
                FixedMonthlyAmount = 2500m,
                EffectiveFrom = default
            })
            .ShouldHaveErrorFor(nameof(CreateSalaryFormulaCommand.EffectiveFrom));

    [Test]
    public void ShouldHaveError_WhenFixedMonthlyFormulaHasNoAmount()
        => _validator.Validate(new CreateSalaryFormulaCommand
            {
                FormulaType = SalaryFormulaType.FixedMonthly,
                FixedMonthlyAmount = null,
                EffectiveFrom = new DateOnly(2026, 1, 1)
            })
            .ShouldHaveErrorFor(nameof(CreateSalaryFormulaCommand.FixedMonthlyAmount));

    [Test]
    public void ShouldHaveError_WhenFixedMonthlyAmountIsNotPositive()
        => _validator.Validate(new CreateSalaryFormulaCommand
            {
                FormulaType = SalaryFormulaType.FixedMonthly,
                FixedMonthlyAmount = 0m,
                EffectiveFrom = new DateOnly(2026, 1, 1)
            })
            .ShouldHaveErrorFor(nameof(CreateSalaryFormulaCommand.FixedMonthlyAmount));

    [Test]
    public void ShouldHaveError_WhenTieredFormulaHasNoTiers()
        => _validator.Validate(new CreateSalaryFormulaCommand
            {
                FormulaType = SalaryFormulaType.TieredPerOrder,
                EffectiveFrom = new DateOnly(2026, 1, 1),
                Tiers = []
            })
            .ShouldHaveErrorFor(nameof(CreateSalaryFormulaCommand.Tiers));

    [Test]
    public void ShouldNotHaveErrors_ForAValidFixedMonthlyFormula()
        => _validator.Validate(new CreateSalaryFormulaCommand
            {
                FormulaType = SalaryFormulaType.FixedMonthly,
                FixedMonthlyAmount = 2500m,
                EffectiveFrom = new DateOnly(2026, 1, 1)
            })
            .IsValid.ShouldBeTrue();

    [Test]
    public void ShouldNotHaveErrors_ForAValidTieredFormula()
        => _validator.Validate(new CreateSalaryFormulaCommand
            {
                FormulaType = SalaryFormulaType.TieredPerOrder,
                EffectiveFrom = new DateOnly(2026, 1, 1),
                Tiers = [new SalaryTierInput(1, null, SalaryTierRateType.PerOrder, 10m)]
            })
            .IsValid.ShouldBeTrue();
}
