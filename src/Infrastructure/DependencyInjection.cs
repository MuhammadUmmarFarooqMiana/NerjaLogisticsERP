using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Services;
using NerjaLogisticsERP.Infrastructure.BackgroundJobs;
using NerjaLogisticsERP.Infrastructure.Data;
using NerjaLogisticsERP.Infrastructure.Data.Interceptors;
using NerjaLogisticsERP.Infrastructure.FileStorage;
using NerjaLogisticsERP.Infrastructure.HealthChecks;
using NerjaLogisticsERP.Infrastructure.Identity;
using NerjaLogisticsERP.Infrastructure.Reports;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(Services.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{Services.Database}' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });

        builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database");

        //builder.Services.AddAuthentication(options =>
        //    {
        //        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        //        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        //    })
        //    .AddIdentityCookies();

        //builder.Services.AddAuthorizationBuilder();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                var jwtKey = builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        builder.Services.AddAuthorization();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        builder.Services.AddHostedService<DailyOrderAutoCloseService>();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
        builder.Services.AddSingleton<ISalaryCalculator, SalaryCalculator>();
        builder.Services.AddScoped<IPlatformReconciliationFileParser, PlatformReconciliationFileParser>();

        // PDFsharp targets net10.0 (not net10.0-windows), so it has no automatic access to
        // system fonts and needs a resolver set once, globally, before any PDF is generated.
        PdfSharp.Fonts.GlobalFontSettings.FontResolver ??= new PdfWindowsFontResolver();

        // Fallback for every report type that doesn't have a real exporter yet — a specific
        // registration below (e.g. IReportExporter<OrdersReportDto>) always wins over this
        // open-generic one, so each report only needs to add its own line here as it ships.
        builder.Services.AddScoped(typeof(IReportExporter<>), typeof(NotImplementedReportExporter<>));
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Orders.OrdersReportDto>, OrdersReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Fines.FinesReportDto>, FinesReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Advances.AdvancesReportDto>, AdvancesReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Expenses.ExpensesReportDto>, ExpensesReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Leaves.LeavesReportDto>, LeavesReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Salaries.SalariesReportDto>, SalariesReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Vehicles.VehiclesReportDto>, VehiclesReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Suppliers.SuppliersReportDto>, SuppliersReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.Inventory.InventoryLedgerReportDto>, InventoryLedgerReportExporter>();
        builder.Services.AddScoped<IReportExporter<NerjaLogisticsERP.Application.Reports.PlatformReconciliation.PlatformReconciliationReportDto>, PlatformReconciliationReportExporter>();
    }
}
