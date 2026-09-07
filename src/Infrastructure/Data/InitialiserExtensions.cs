using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace NerjaLogisticsERP.Infrastructure.Data;

public static class InitialiserExtensions
{
    /// <summary>
    /// Applies pending EF Core migrations. Called unconditionally in every environment —
    /// without it, a freshly-provisioned production database has no schema and every
    /// request fails.
    /// </summary>
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
    }

    /// <summary>
    /// Creates the app's fixed roles (Administrator, SoftwareEngineer, Supervisor,
    /// Accountant, Rider) if they don't already exist yet. Called unconditionally in every
    /// environment, unlike <see cref="SeedDevelopmentAdministratorAsync"/> — roles carry no
    /// secrets, and role-based authorization can't work at all until they exist as real rows,
    /// production included.
    /// </summary>
    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.SeedRolesAsync();
    }

    /// <summary>
    /// Creates the built-in Administrator account. Its password is a hardcoded, publicly-known
    /// default — safe for local development, but this must never be wired up to run unattended
    /// in production. Bootstrap the real first production admin as a deliberate one-off step
    /// instead (e.g. register normally through the API, then promote that account).
    /// </summary>
    public static async Task SeedDevelopmentAdministratorAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.SeedDevelopmentAdministratorAsync();
    }
}
