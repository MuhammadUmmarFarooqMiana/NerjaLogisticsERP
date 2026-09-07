using NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.DailyOrders;

public class UpsertDailyOrderCommandValidatorTests
{
    private readonly UpsertDailyOrderCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenCompletedOrdersIsNegative()
        => _validator.Validate(new UpsertDailyOrderCommand { CompletedOrders = -1 })
            .ShouldHaveErrorFor(nameof(UpsertDailyOrderCommand.CompletedOrders));

    [TestCase(0)]
    [TestCase(50)]
    public void ShouldNotHaveErrors_WhenCompletedOrdersIsZeroOrPositive(int completedOrders)
        => _validator.Validate(new UpsertDailyOrderCommand { CompletedOrders = completedOrders })
            .IsValid.ShouldBeTrue();
}
