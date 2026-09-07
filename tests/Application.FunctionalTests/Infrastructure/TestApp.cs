using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Infrastructure.Data;
using NerjaLogisticsERP.Infrastructure.Identity;

namespace NerjaLogisticsERP.Application.FunctionalTests.Infrastructure;

public static class TestApp
{
    private static string? _userId;
    private static List<string>? _roles;
    private static List<PlatformRiderRow>? _reconciliationRows;

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task SendAsync(IBaseRequest request)
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        await mediator.Send(request);
    }

    public static string? GetUserId() => _userId;

    public static List<string>? GetRoles() => _roles;

    /// <summary>
    /// Switches the identity SendAsync acts as to an already-created user, without creating a
    /// new Identity user (RunAsUserAsync can't be called twice for the same user — CreateAsync
    /// would fail on the duplicate username). For tests that set up more than one user (e.g. a
    /// rider and their supervisor) and need to alternate which one is "current" across multiple
    /// SendAsync calls in the same test.
    /// </summary>
    public static void RunAs(Guid userId, IEnumerable<string> roles)
    {
        _userId = userId.ToString();
        _roles = [.. roles];
    }

    public static async Task<string> RunAsDefaultUserAsync()
    {
        return await RunAsUserAsync("test@local", "Testing1234!", []);
    }

    public static async Task<string> RunAsAdministratorAsync()
    {
        return await RunAsUserAsync("administrator@nerja", "Administrator1234!", [Roles.Administrator]);
    }

    public static async Task<string> RunAsUserAsync(string userName, string password, string[] roles)
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser { UserName = userName, Email = userName };

        var result = await userManager.CreateAsync(user, password);

        if (roles.Length > 0)
        {
            // IdentityRole<Guid>, not the bare (string-keyed) IdentityRole — this app registers
            // roles via AddRoles<IdentityRole<Guid>>() (see Infrastructure/DependencyInjection.cs),
            // so RoleManager<IdentityRole> simply isn't a registered service. IdentityRole<Guid>'s
            // Id also isn't auto-generated the way the string-keyed IdentityRole's is, so it has
            // to be set explicitly or every role ends up with Id == Guid.Empty.
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var role in roles)
            {
                // Respawn only resets between tests, not between calls within one test — a single
                // test creating two users with an overlapping role (e.g. two Supervisors, to test
                // "not your report") would otherwise try to create the same role name twice.
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = role });
                }
            }

            await userManager.AddToRolesAsync(user, roles);
        }

        if (result.Succeeded)
        {
            _userId = user.Id.ToString();
            _roles = [.. roles];
            return _userId;
        }

        var errors = string.Join(Environment.NewLine, result.ToApplicationResult().Errors);

        throw new Exception($"Unable to create {userName}.{Environment.NewLine}{errors}");
    }

    public static async Task ResetState()
    {
        if (FunctionalTestSetup.DbResetter is not null)
        {
            await FunctionalTestSetup.DbResetter.ResetAsync();
        }

        _userId = null;
        _roles = null;
        _reconciliationRows = null;
    }

    /// <summary>The fake IPlatformReconciliationFileParser (see WebApiFactory) returns these
    /// instead of actually parsing bytes — set before sending a GenerateReconciliationReportCommand.</summary>
    public static void SetReconciliationRows(List<PlatformRiderRow> rows) => _reconciliationRows = rows;

    public static List<PlatformRiderRow> GetReconciliationRows() => _reconciliationRows ?? [];

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Persists further mutations made (via domain methods) to an entity that was already
    /// saved earlier in the test — e.g. calling employee.Terminate() to simulate an employee
    /// being terminated partway through a test, after they already logged in. The entity is
    /// detached (it came from a disposed scope, or was built in memory), so this attaches it
    /// to a fresh context and marks it Modified rather than trying to reuse a tracked instance.
    /// </summary>
    public static async Task UpdateAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Update(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = FunctionalTestSetup.ScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await context.Set<TEntity>().CountAsync();
    }
}
