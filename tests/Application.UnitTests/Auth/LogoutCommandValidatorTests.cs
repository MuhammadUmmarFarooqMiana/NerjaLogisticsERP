using NerjaLogisticsERP.Application.Auth.Commands.Logout;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Auth;

public class LogoutCommandValidatorTests
{
    private readonly LogoutCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenRefreshTokenIsEmpty()
        => _validator.Validate(new LogoutCommand { RefreshToken = "" })
            .ShouldHaveErrorFor(nameof(LogoutCommand.RefreshToken));

    [Test]
    public void ShouldNotHaveErrors_WhenRefreshTokenIsProvided()
        => _validator.Validate(new LogoutCommand { RefreshToken = "some-token-value" })
            .IsValid.ShouldBeTrue();
}
