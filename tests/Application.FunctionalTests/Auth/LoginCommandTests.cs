using NerjaLogisticsERP.Application.Auth.Commands.Login;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.Auth;

public class LoginCommandTests : TestBase
{
    [Test]
    public async Task ShouldReturnTokens_WhenCredentialsAreValidAndEmployeeIsActive()
    {
        var (email, _) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Active);

        var result = await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword });

        result.AccessToken.ShouldNotBeNullOrWhiteSpace();
        result.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        result.ExpiresAt.ShouldBeGreaterThan(DateTimeOffset.UtcNow);
    }

    // Incomplete/PendingReview/Rejected riders must still be able to log in — that's how they
    // reach the screen where they submit or correct their profile in the first place. Only
    // Suspended/Terminated actually block login (see the two Forbidden tests below).
    [Test]
    public async Task ShouldReturnTokens_WhenEmployeeIsPendingReview()
    {
        var (email, _) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.PendingReview);

        var result = await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword });

        result.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ShouldThrowUnauthorized_WhenPasswordIsWrong()
    {
        var (email, _) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Active);

        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync(new LoginCommand { Email = email, Password = "WrongPassword1!" }));
    }

    [Test]
    public async Task ShouldThrowUnauthorized_WhenEmailDoesNotExist()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync(new LoginCommand { Email = "nobody@test.local", Password = "Whatever123!" }));
    }

    [Test]
    public async Task ShouldThrowForbidden_WhenEmployeeIsSuspended()
    {
        var (email, _) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Suspended);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword }));
    }

    [Test]
    public async Task ShouldThrowForbidden_WhenEmployeeIsTerminated()
    {
        var (email, _) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Terminated);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword }));
    }

    // Credentials can be valid at the Identity level with no matching Employee row at all
    // (defensive case — shouldn't happen via the normal Register flow, but the handler
    // explicitly checks `employee is null` before checking status, so it's worth pinning down).
    [Test]
    public async Task ShouldThrowForbidden_WhenNoEmployeeRecordExists()
    {
        const string email = "identity-only@test.local";
        await TestApp.RunAsUserAsync(email, AuthTestHelpers.DefaultPassword, []);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword }));
    }
}
