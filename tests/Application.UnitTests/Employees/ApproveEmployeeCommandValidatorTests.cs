using NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Employees;

public class ApproveEmployeeCommandValidatorTests
{
    private readonly ApproveEmployeeCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenEmployeeIdIsEmpty()
        => _validator.Validate(new ApproveEmployeeCommand { EmployeeId = Guid.Empty })
            .ShouldHaveErrorFor(nameof(ApproveEmployeeCommand.EmployeeId));

    [Test]
    public void ShouldNotHaveErrors_WhenEmployeeIdIsProvided()
        => _validator.Validate(new ApproveEmployeeCommand { EmployeeId = Guid.NewGuid() })
            .IsValid.ShouldBeTrue();
}
