using NerjaLogisticsERP.Application.Employees.Commands.RejectEmployee;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Employees;

public class RejectEmployeeCommandValidatorTests
{
    private readonly RejectEmployeeCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(new RejectEmployeeCommand { EmployeeId = Guid.Empty, Reason = "Incomplete documents." })
            .ShouldHaveErrorFor(nameof(RejectEmployeeCommand.EmployeeId));

    [Test]
    public void ShouldHaveError_WhenReasonIsEmpty()
        => _validator.Validate(new RejectEmployeeCommand { EmployeeId = Guid.NewGuid(), Reason = "" })
            .ShouldHaveErrorFor(nameof(RejectEmployeeCommand.Reason));

    [Test]
    public void ShouldHaveError_WhenReasonExceedsMaxLength()
        => _validator.Validate(new RejectEmployeeCommand { EmployeeId = Guid.NewGuid(), Reason = new string('x', 5001) })
            .ShouldHaveErrorFor(nameof(RejectEmployeeCommand.Reason));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRejection()
        => _validator.Validate(new RejectEmployeeCommand { EmployeeId = Guid.NewGuid(), Reason = "Incomplete documents." })
            .IsValid.ShouldBeTrue();
}
