using NerjaLogisticsERP.Application.LeaveRequest.Commands.SubmitLeaveRequest;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.LeaveRequest;

public class SubmitLeaveRequestCommandValidatorTests
{
    private readonly SubmitLeaveRequestCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenEndDateIsBeforeStartDate()
        => _validator.Validate(new SubmitLeaveRequestCommand
            {
                StartDate = new DateOnly(2026, 7, 10),
                EndDate = new DateOnly(2026, 7, 5)
            })
            .ShouldHaveErrorFor(nameof(SubmitLeaveRequestCommand.EndDate));

    [Test]
    public void ShouldNotHaveErrors_WhenEndDateEqualsStartDate()
        => _validator.Validate(new SubmitLeaveRequestCommand
            {
                StartDate = new DateOnly(2026, 7, 10),
                EndDate = new DateOnly(2026, 7, 10)
            })
            .IsValid.ShouldBeTrue();

    [Test]
    public void ShouldNotHaveErrors_WhenEndDateIsAfterStartDate()
        => _validator.Validate(new SubmitLeaveRequestCommand
            {
                StartDate = new DateOnly(2026, 7, 10),
                EndDate = new DateOnly(2026, 7, 15)
            })
            .IsValid.ShouldBeTrue();
}
