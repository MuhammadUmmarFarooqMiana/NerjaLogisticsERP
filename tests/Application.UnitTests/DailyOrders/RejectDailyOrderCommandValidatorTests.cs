using NerjaLogisticsERP.Application.DailyOrders.Commands.RejectDailyOrder;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.DailyOrders;

public class RejectDailyOrderCommandValidatorTests
{
    private readonly RejectDailyOrderCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenReasonIsEmpty()
        => _validator.Validate(new RejectDailyOrderCommand { DailyOrderId = Guid.NewGuid(), Reason = "" })
            .ShouldHaveErrorFor(nameof(RejectDailyOrderCommand.Reason));

    [Test]
    public void ShouldHaveError_WhenReasonExceedsMaxLength()
        => _validator.Validate(new RejectDailyOrderCommand { DailyOrderId = Guid.NewGuid(), Reason = new string('x', 5001) })
            .ShouldHaveErrorFor(nameof(RejectDailyOrderCommand.Reason));

    [Test]
    public void ShouldNotHaveErrors_WhenReasonIsProvided()
        => _validator.Validate(new RejectDailyOrderCommand { DailyOrderId = Guid.NewGuid(), Reason = "Count looks wrong." })
            .IsValid.ShouldBeTrue();
}
