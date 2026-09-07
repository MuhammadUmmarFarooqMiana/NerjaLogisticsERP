using NerjaLogisticsERP.Application.Auth.Commands.Login;
using NerjaLogisticsERP.Application.Auth.Commands.RefreshToken;
using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.FunctionalTests.Auth;

public class RefreshCommandTests : TestBase
{
    // Both remaining callers only ever need an Active employee's token — the Suspended/
    // Terminated tests below can't use this at all, since they need to mutate the employee's
    // status *after* logging in, not before (see their own comments).
    private static async Task<string> LoginAndGetRefreshTokenAsync()
    {
        var (email, _) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Active);
        var login = await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword });
        return login.RefreshToken;
    }

    [Test]
    public async Task ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        var refreshToken = await LoginAndGetRefreshTokenAsync();

        var result = await TestApp.SendAsync(new RefreshTokenCommand { RefreshToken = refreshToken });

        result.AccessToken.ShouldNotBeNullOrWhiteSpace();
        result.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        result.RefreshToken.ShouldNotBe(refreshToken); // rotated, not reissued as-is
    }

    [Test]
    public async Task ShouldThrowUnauthorized_WhenRefreshTokenIsUnknown()
    {
        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync(new RefreshTokenCommand { RefreshToken = "not-a-real-token" }));
    }

    // ValidateAndRotateAsync revokes the token the moment it's used, so the same raw refresh
    // token can't be redeemed twice — a second use (e.g. a stolen/replayed token, or a retried
    // request) must fail rather than silently hand out another pair of tokens.
    [Test]
    public async Task ShouldThrowUnauthorized_WhenRefreshTokenIsReused()
    {
        var refreshToken = await LoginAndGetRefreshTokenAsync();

        await TestApp.SendAsync(new RefreshTokenCommand { RefreshToken = refreshToken }); // first use: consumes it

        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync(new RefreshTokenCommand { RefreshToken = refreshToken })); // second use: already revoked
    }

    // Deliberately differs from Login: RefreshCommandHandler only blocks Terminated, not
    // Suspended, so a Suspended employee's existing session keeps refreshing rather than being
    // killed mid-session — see the handler's own comment on why. Login itself blocks Suspended
    // too, so (as with the Terminated test above) this has to suspend the employee *after*
    // they've already logged in, not try to log a Suspended employee in fresh.
    [Test]
    public async Task ShouldSucceed_WhenEmployeeIsSuspendedAfterLoggingIn()
    {
        var (email, employeeId) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Active);
        var login = await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword });

        var employee = await TestApp.FindAsync<Employee>(employeeId)
            ?? throw new InvalidOperationException("Seeded employee not found.");
        employee.Suspend();
        await TestApp.UpdateAsync(employee);

        var result = await TestApp.SendAsync(new RefreshTokenCommand { RefreshToken = login.RefreshToken });

        result.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    // Login itself already blocks Terminated employees, so the only way to reach this state is
    // an employee who was Active (and logged in) getting terminated afterward, mid-session —
    // that's the scenario this reproduces, rather than trying to log a Terminated employee in.
    [Test]
    public async Task ShouldThrowForbidden_WhenEmployeeIsTerminatedAfterLoggingIn()
    {
        var (email, employeeId) = await AuthTestHelpers.CreateLoginableEmployeeAsync(AccountStatus.Active);
        var login = await TestApp.SendAsync(new LoginCommand { Email = email, Password = AuthTestHelpers.DefaultPassword });

        var employee = await TestApp.FindAsync<Employee>(employeeId)
            ?? throw new InvalidOperationException("Seeded employee not found.");
        employee.Terminate();
        await TestApp.UpdateAsync(employee);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new RefreshTokenCommand { RefreshToken = login.RefreshToken }));
    }
}
