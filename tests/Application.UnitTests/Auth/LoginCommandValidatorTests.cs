using NerjaLogisticsERP.Application.Auth.Commands.Login;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Auth;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenEmailIsEmpty()
        => _validator.Validate(new LoginCommand { Email = "", Password = "password1" })
            .ShouldHaveErrorFor(nameof(LoginCommand.Email));

    [Test]
    public void ShouldHaveError_WhenEmailIsNotAValidAddress()
        => _validator.Validate(new LoginCommand { Email = "not-an-email", Password = "password1" })
            .ShouldHaveErrorFor(nameof(LoginCommand.Email));

    [TestCase("")]
    [TestCase("abc")] // shorter than the 6-character minimum
    public void ShouldHaveError_WhenPasswordIsTooShort(string password)
        => _validator.Validate(new LoginCommand { Email = "test@nerja.com", Password = password })
            .ShouldHaveErrorFor(nameof(LoginCommand.Password));

    [Test]
    public void ShouldNotHaveErrors_ForValidCredentials()
        => _validator.Validate(new LoginCommand { Email = "test@nerja.com", Password = "password1" })
            .IsValid.ShouldBeTrue();
}
