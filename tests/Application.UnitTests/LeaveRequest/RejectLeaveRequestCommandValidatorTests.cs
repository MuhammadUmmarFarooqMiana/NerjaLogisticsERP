using NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.LeaveRequest;

public class RejectLeaveRequestCommandValidatorTests
{
    private readonly RejectLeaveRequestCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenReasonIsEmpty()
        => _validator.Validate(new RejectLeaveRequestCommand { Reason = "" })
            .ShouldHaveErrorFor(nameof(RejectLeaveRequestCommand.Reason));

    [Test]
    public void ShouldHaveError_WhenReasonExceedsMaxLength()
        => _validator.Validate(new RejectLeaveRequestCommand { Reason = new string('x', 5001) })
            .ShouldHaveErrorFor(nameof(RejectLeaveRequestCommand.Reason));

    [Test]
    public void ShouldNotHaveErrors_WhenReasonIsProvided()
        => _validator.Validate(new RejectLeaveRequestCommand { Reason = "Insufficient balance." })
            .IsValid.ShouldBeTrue();
}
