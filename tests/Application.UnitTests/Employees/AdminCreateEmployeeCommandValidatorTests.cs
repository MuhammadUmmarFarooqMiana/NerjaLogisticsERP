using NerjaLogisticsERP.Application.Employees.Commands.AdminCreateEmployee;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Employees;

public class AdminCreateEmployeeCommandValidatorTests
{
    private readonly AdminCreateEmployeeCommandValidator _validator = new();

    private static AdminCreateEmployeeCommand Valid() => new()
    {
        FullName = "Test Rider",
        Email = "rider@test.local",
        Password = "pass12",
        PhoneNumber = "+966501234567",
        Roles = ["Rider"],
        IqamaNumber = "1234567890"
    };

    [Test]
    public void ShouldHaveError_WhenFullNameExceedsMaxLength()
        => _validator.Validate(Valid() with { FullName = new string('x', 201) })
            .ShouldHaveErrorFor(nameof(AdminCreateEmployeeCommand.FullName));

    [Test]
    public void ShouldHaveError_WhenEmailIsNotValid()
        => _validator.Validate(Valid() with { Email = "not-an-email" })
            .ShouldHaveErrorFor(nameof(AdminCreateEmployeeCommand.Email));

    [Test]
    public void ShouldHaveError_WhenPasswordIsShorterThanSixCharacters()
        => _validator.Validate(Valid() with { Password = "12345" })
            .ShouldHaveErrorFor(nameof(AdminCreateEmployeeCommand.Password));

    [Test]
    public void ShouldHaveError_WhenPhoneNumberIsEmpty()
        => _validator.Validate(Valid() with { PhoneNumber = "" })
            .ShouldHaveErrorFor(nameof(AdminCreateEmployeeCommand.PhoneNumber));

    [Test]
    public void ShouldHaveError_WhenNoRolesAreProvided()
        => _validator.Validate(Valid() with { Roles = [] })
            .ShouldHaveErrorFor(nameof(AdminCreateEmployeeCommand.Roles));

    [Test]
    public void ShouldHaveError_WhenIqamaNumberIsEmpty()
        => _validator.Validate(Valid() with { IqamaNumber = "" })
            .ShouldHaveErrorFor(nameof(AdminCreateEmployeeCommand.IqamaNumber));

    [Test]
    public void ShouldHaveError_WhenPlatformIsSetButPlatformIdNumberIsMissing()
        => _validator.Validate(Valid() with { PlatformId = Guid.NewGuid(), PlatformIdNumber = null })
            .ShouldHaveErrorFor(nameof(AdminCreateEmployeeCommand.PlatformIdNumber));

    [Test]
    public void ShouldNotHaveError_ForPlatformIdNumber_WhenNoPlatformIsSet()
        => _validator.Validate(Valid() with { PlatformId = null, PlatformIdNumber = null })
            .ShouldNotHaveErrorFor(nameof(AdminCreateEmployeeCommand.PlatformIdNumber));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRequest()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
