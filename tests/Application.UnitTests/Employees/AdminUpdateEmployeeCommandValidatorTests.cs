using NerjaLogisticsERP.Application.Employees.Commands.AdminUpdateEmployee;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Employees;

public class AdminUpdateEmployeeCommandValidatorTests
{
    private readonly AdminUpdateEmployeeCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenIdIsEmpty()
        => _validator.Validate(new AdminUpdateEmployeeCommand { Id = Guid.Empty, FullName = "Test Rider" })
            .ShouldHaveErrorFor(nameof(AdminUpdateEmployeeCommand.Id));

    [Test]
    public void ShouldHaveError_WhenFullNameIsEmpty()
        => _validator.Validate(new AdminUpdateEmployeeCommand { Id = Guid.NewGuid(), FullName = "" })
            .ShouldHaveErrorFor(nameof(AdminUpdateEmployeeCommand.FullName));

    [Test]
    public void ShouldHaveError_WhenFullNameExceedsMaxLength()
        => _validator.Validate(new AdminUpdateEmployeeCommand { Id = Guid.NewGuid(), FullName = new string('x', 201) })
            .ShouldHaveErrorFor(nameof(AdminUpdateEmployeeCommand.FullName));

    [Test]
    public void ShouldNotHaveErrors_ForAValidUpdate()
        => _validator.Validate(new AdminUpdateEmployeeCommand { Id = Guid.NewGuid(), FullName = "Test Rider" })
            .IsValid.ShouldBeTrue();
}
