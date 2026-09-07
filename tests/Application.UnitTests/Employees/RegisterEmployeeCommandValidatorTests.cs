using NerjaLogisticsERP.Application.Employees.Commands.RegisterEmployee;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Employees;

public class RegisterEmployeeCommandValidatorTests
{
    private readonly RegisterEmployeeCommandValidator _validator = new();

    private static RegisterEmployeeCommand Valid() => new()
    {
        FullName = "Test Rider",
        Email = "rider@test.local",
        Password = "password1",
        PhoneNumber = "+966501234567"
    };

    [Test]
    public void ShouldHaveError_WhenFullNameIsEmpty()
        => _validator.Validate(Valid() with { FullName = "" })
            .ShouldHaveErrorFor(nameof(RegisterEmployeeCommand.FullName));

    [Test]
    public void ShouldHaveError_WhenFullNameExceedsMaxLength()
        => _validator.Validate(Valid() with { FullName = new string('x', 151) })
            .ShouldHaveErrorFor(nameof(RegisterEmployeeCommand.FullName));

    [Test]
    public void ShouldHaveError_WhenEmailIsNotValid()
        => _validator.Validate(Valid() with { Email = "not-an-email" })
            .ShouldHaveErrorFor(nameof(RegisterEmployeeCommand.Email));

    [Test]
    public void ShouldHaveError_WhenPasswordIsShorterThanEightCharacters()
        => _validator.Validate(Valid() with { Password = "short1" })
            .ShouldHaveErrorFor(nameof(RegisterEmployeeCommand.Password));

    [TestCase("")]
    [TestCase("12345")] // fewer than 9 digits
    [TestCase("not-a-number")]
    public void ShouldHaveError_WhenPhoneNumberIsInvalid(string phoneNumber)
        => _validator.Validate(Valid() with { PhoneNumber = phoneNumber })
            .ShouldHaveErrorFor(nameof(RegisterEmployeeCommand.PhoneNumber));

    [Test]
    public void ShouldNotHaveErrors_ForAValidRegistration()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
