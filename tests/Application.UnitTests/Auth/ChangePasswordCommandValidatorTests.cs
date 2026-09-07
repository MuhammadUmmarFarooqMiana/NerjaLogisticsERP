using NerjaLogisticsERP.Application.Auth.Commands.ChangePassword;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Auth;

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    private static ChangePasswordCommand Valid() => new()
    {
        CurrentPassword = "OldPass1!",
        NewPassword = "NewPass1!",
        ConfirmNewPassword = "NewPass1!"
    };

    [Test]
    public void ShouldHaveError_WhenCurrentPasswordIsEmpty()
        => _validator.Validate(Valid() with { CurrentPassword = "" })
            .ShouldHaveErrorFor(nameof(ChangePasswordCommand.CurrentPassword));

    [Test]
    public void ShouldHaveError_WhenNewPasswordIsShorterThanEightCharacters()
        => _validator.Validate(Valid() with { NewPassword = "Sh0rt!", ConfirmNewPassword = "Sh0rt!" })
            .ShouldHaveErrorFor(nameof(ChangePasswordCommand.NewPassword));

    [TestCase("lowercase1!")] // no uppercase
    [TestCase("UPPERCASE1!")] // no lowercase
    [TestCase("NoDigitsHere!")] // no digit
    [TestCase("NoSymbolsHere1")] // no symbol
    public void ShouldHaveError_WhenNewPasswordFailsComplexityRule(string newPassword)
        => _validator.Validate(Valid() with { NewPassword = newPassword, ConfirmNewPassword = newPassword })
            .ShouldHaveErrorFor(nameof(ChangePasswordCommand.NewPassword));

    [Test]
    public void ShouldHaveError_WhenConfirmNewPasswordDoesNotMatchNewPassword()
        => _validator.Validate(Valid() with { ConfirmNewPassword = "SomethingElse1!" })
            .ShouldHaveErrorFor(nameof(ChangePasswordCommand.ConfirmNewPassword));

    [Test]
    public void ShouldHaveError_WhenNewPasswordEqualsCurrentPassword()
        => _validator.Validate(Valid() with { NewPassword = "OldPass1!", ConfirmNewPassword = "OldPass1!", CurrentPassword = "OldPass1!" })
            .ShouldHaveErrorFor(nameof(ChangePasswordCommand.NewPassword));

    [Test]
    public void ShouldNotHaveErrors_ForAValidPasswordChange()
        => _validator.Validate(Valid())
            .IsValid.ShouldBeTrue();
}
