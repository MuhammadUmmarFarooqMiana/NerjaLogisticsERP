using NerjaLogisticsERP.Application.Auth.Commands.Login;
using NerjaLogisticsERP.Application.Auth.Commands.Logout;
using NerjaLogisticsERP.Application.Auth.Commands.RefreshToken;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.Auth;

public class LogoutCommandTests : TestBase
{
    // The real point of logout: the refresh token must actually stop working afterward, not
    // just return a success response. Verified end-to-end by trying to refresh with it after.
    [Test]
    public async Task ShouldRevokeRefreshToken_SoItCanNoLongerBeUsedToRefresh()
    {
        var (email, _) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Active);
        var login = await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword });

        await TestApp.SendAsync(new LogoutCommand { RefreshToken = login.RefreshToken });

        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync(new RefreshTokenCommand { RefreshToken = login.RefreshToken }));
    }

    // RefreshTokenService.RevokeAsync no-ops on a token it doesn't recognise (see the
    // implementation) — logging out is idempotent/best-effort, not an error if the token was
    // already revoked or never existed.
    [Test]
    public async Task ShouldNotThrow_WhenRefreshTokenIsUnknown()
    {
        await Should.NotThrowAsync(async () =>
            await TestApp.SendAsync(new LogoutCommand { RefreshToken = "not-a-real-token" }));
    }
}
