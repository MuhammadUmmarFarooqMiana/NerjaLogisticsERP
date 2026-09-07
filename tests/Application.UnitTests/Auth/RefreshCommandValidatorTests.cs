using NerjaLogisticsERP.Application.Auth.Commands.RefreshToken;
using NerjaLogisticsERP.Application.UnitTests.TestHelpers;
using NUnit.Framework;
using Shouldly;

namespace NerjaLogisticsERP.Application.UnitTests.Auth;

public class RefreshCommandValidatorTests
{
    private readonly RefreshCommandValidator _validator = new();

    [Test]
    public void ShouldHaveError_WhenRefreshTokenIsEmpty()
        => _validator.Validate(new RefreshTokenCommand { RefreshToken = "" })
            .ShouldHaveErrorFor(nameof(RefreshTokenCommand.RefreshToken));

    [Test]
    public void ShouldNotHaveErrors_WhenRefreshTokenIsProvided()
        => _validator.Validate(new RefreshTokenCommand { RefreshToken = "some-token-value" })
            .IsValid.ShouldBeTrue();
}
