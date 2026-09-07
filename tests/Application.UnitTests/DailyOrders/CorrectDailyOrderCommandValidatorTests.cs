using NerjaLogisticsERP.Application.DailyOrders.Commands.CorrectDailyOrder;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.DailyOrders;

public class CorrectDailyOrderCommandValidatorTests
{
    private readonly CorrectDailyOrderCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenCompletedOrdersIsNegative()
        => _validator.Validate(new CorrectDailyOrderCommand { DailyOrderId = Guid.NewGuid(), CompletedOrders = -1, Reason = "Fixed." })
            .ShouldHaveErrorFor(nameof(CorrectDailyOrderCommand.CompletedOrders));

    [Test]
    public void ShouldHaveError_WhenReasonIsEmpty()
        => _validator.Validate(new CorrectDailyOrderCommand { DailyOrderId = Guid.NewGuid(), CompletedOrders = 10, Reason = "" })
            .ShouldHaveErrorFor(nameof(CorrectDailyOrderCommand.Reason));

    [Test]
    public void ShouldHaveError_WhenReasonExceedsMaxLength()
        => _validator.Validate(new CorrectDailyOrderCommand { DailyOrderId = Guid.NewGuid(), CompletedOrders = 10, Reason = new string('x', 5001) })
            .ShouldHaveErrorFor(nameof(CorrectDailyOrderCommand.Reason));

    [Test]
    public void ShouldNotHaveErrors_ForAValidCorrection()
        => _validator.Validate(new CorrectDailyOrderCommand { DailyOrderId = Guid.NewGuid(), CompletedOrders = 10, Reason = "Verified against the platform export." })
            .IsValid.ShouldBeTrue();
}
