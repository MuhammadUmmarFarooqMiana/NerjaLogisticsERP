using NerjaLogisticsERP.Application.Salaries.Commands.CreateSalaryFormula;
using NerjaLogisticsERP.Application.Salaries.Commands.UpdateSalaryFormula;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NerjaLogisticsERP.Domain.Enums;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Salaries;

public class UpdateSalaryFormulaCommandValidatorTests
{
    private readonly UpdateSalaryFormulaCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(new UpdateSalaryFormulaCommand { Id = Guid.Empty })
            .ShouldHaveErrorFor(nameof(UpdateSalaryFormulaCommand.Id));

    [Test]
    public void ShouldHaveError_WhenFixedMonthlyAmountIsProvidedButNotPositive()
        => _validator.Validate(new UpdateSalaryFormulaCommand { Id = Guid.NewGuid(), FixedMonthlyAmount = 0m })
            .ShouldHaveErrorFor(nameof(UpdateSalaryFormulaCommand.FixedMonthlyAmount));

    [Test]
    public void ShouldNotHaveError_WhenFixedMonthlyAmountIsNull()
        => _validator.Validate(new UpdateSalaryFormulaCommand { Id = Guid.NewGuid(), FixedMonthlyAmount = null })
            .ShouldNotHaveErrorFor(nameof(UpdateSalaryFormulaCommand.FixedMonthlyAmount));

    [Test]
    public void ShouldHaveError_WhenATierHasANegativeMinOrders()
        => _validator.Validate(new UpdateSalaryFormulaCommand
            {
                Id = Guid.NewGuid(),
                Tiers = [new SalaryTierInput(-1, null, SalaryTierRateType.PerOrder, 10m)]
            })
            .IsValid.ShouldBeFalse();

    [Test]
    public void ShouldHaveError_WhenATierHasANegativeRate()
        => _validator.Validate(new UpdateSalaryFormulaCommand
            {
                Id = Guid.NewGuid(),
                Tiers = [new SalaryTierInput(1, null, SalaryTierRateType.PerOrder, -1m)]
            })
            .IsValid.ShouldBeFalse();

    [Test]
    public void ShouldHaveError_WhenATierMaxOrdersIsLessThanMinOrders()
        => _validator.Validate(new UpdateSalaryFormulaCommand
            {
                Id = Guid.NewGuid(),
                Tiers = [new SalaryTierInput(50, 10, SalaryTierRateType.PerOrder, 10m)]
            })
            .IsValid.ShouldBeFalse();

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(new UpdateSalaryFormulaCommand
            {
                Id = Guid.NewGuid(),
                FixedMonthlyAmount = 2500m,
                Tiers = [new SalaryTierInput(1, 50, SalaryTierRateType.PerOrder, 10m)]
            })
            .IsValid.ShouldBeTrue();
}
