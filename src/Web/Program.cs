using NerjaLogisticsERP.Infrastructure.Data;
using NerjaLogisticsERP.Web.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Console always (unchanged); a file sink is added on top only when Logging:FilePath is
// configured (set in appsettings.Production.json, left unset in dev so nothing writes to
// disk locally). Custom provider rather than a logging package — see FileLoggerProvider's
// own remarks for why. Registered alongside the existing OpenTelemetry logging provider (see
// ServiceDefaults), not in place of it, so OTLP export still works unchanged if
// OTEL_EXPORTER_OTLP_ENDPOINT is ever configured.
var logFilePath = builder.Configuration["Logging:FilePath"];
if (!string.IsNullOrWhiteSpace(logFilePath))
{
    builder.Logging.AddProvider(new FileLoggerProvider(logFilePath));
}

// Add services to the container.
builder.AddServiceDefaults();

builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Both applied in every environment: a freshly-provisioned production database has no
// schema without the migration, and role-based [Authorize] checks can't pass for anyone
// without the roles existing as real rows. The seeded ADMIN ACCOUNT is the one part that
// stays Development-only — see SeedDevelopmentAdministratorAsync's remarks on why it must
// not run unattended in production.
await app.MigrateDatabaseAsync();
await app.SeedRolesAsync();

// Defaults to true (today's behavior — dev's launchSettings always has an HTTPS endpoint
// available). Set Https:RedirectEnabled=false in appsettings.Production.json until the
// server has a real TLS certificate in front of it — HSTS in particular tells browsers to
// *only* ever use HTTPS for this host from then on, which would make an HTTP-only deployment
// unreachable on the very next visit.
var httpsRedirectEnabled = app.Configuration.GetValue("Https:RedirectEnabled", true);

if (app.Environment.IsDevelopment())
{
    await app.SeedDevelopmentAdministratorAsync();
}
else if (httpsRedirectEnabled)
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (httpsRedirectEnabled)
{
    app.UseHttpsRedirection();
}

// Applied to every response, error pages included, since it's registered before
// UseExceptionHandler. Cheap, standard baseline hardening with no functional downside for
// this app: nosniff stops a browser from executing an uploaded file as a different content
// type than what the server declared (relevant since AllowedFileTypes.IsAllowed only checks
// the client-supplied Content-Type/extension, not the file's actual bytes); DENY is safe
// because nothing here is meant to be iframed; the referrer policy just avoids leaking full
// URLs (which can carry route params like ids) to a cross-origin request's Referer header.
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

// Registered before UseCors/UseAuthentication/UseAuthorization so it also catches exceptions
// thrown by those stages (a misconfigured auth scheme, a CORS policy bug, etc.) — positioned
// after them, it would only ever see exceptions from MVC action execution.
//
// Must be the parameterless overload: UseExceptionHandler(options => { }) binds to the
// Action<IApplicationBuilder> overload and configures an empty re-execution branch, so any
// exception ProblemDetailsExceptionHandler doesn't recognise 404s inside that empty branch
// instead of reaching AddProblemDetails()'s fallback — which crashes with a bare, bodyless 500.
app.UseExceptionHandler();

// Auth failures (missing/expired JWT → 401, valid token but wrong role → 403) are written by
// JwtBearerHandler/the authorization middleware directly — they set the status code and return
// without a body. UseStatusCodePages catches any error-status response that still has an empty
// body at the end of the pipeline and fills it in via the same IProblemDetailsService (and
// traceId customization) as every other error response, instead of leaving it bare. Placed before
// UseAuthentication/UseAuthorization so it wraps them.
app.UseStatusCodePages();

// Empty (the dev default — nothing set in appsettings.json) falls back to AllowAnyOrigin,
// same as before. appsettings.Production.json sets Cors:AllowedOrigins explicitly, so
// production is restricted to just the real deployment origin(s) once configured — since the
// SPA is served same-origin (UseFileServer below), this only affects *other* origins calling
// the API, never the app's own frontend.
var allowedOrigins = app.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

app.UseCors(corsBuilder =>
{
    corsBuilder.AllowAnyMethod()
        .AllowAnyHeader()
        // AllowAnyHeader only covers request headers — response headers need explicit
        // exposure or browser JS can't read them cross-origin, even with AllowAnyOrigin.
        .WithExposedHeaders("X-Pagination");

    if (allowedOrigins.Length > 0)
        corsBuilder.WithOrigins(allowedOrigins);
    else
        corsBuilder.AllowAnyOrigin();
});

app.UseAuthentication();
app.UseAuthorization();

// Must come after UseAuthorization (matches the documented ASP.NET Core middleware order)
// so a rejected-request short-circuit still runs through auth first.
app.UseRateLimiter();

app.UseFileServer();

app.MapOpenApi();
app.MapScalarApiReference();

// Anonymous by design — a load balancer / uptime monitor needs to reach this without a
// token. Response body is just the default Healthy/Unhealthy text, no details attached,
// so it doesn't leak anything beyond "the database is/isn't reachable".
app.MapHealthChecks("/health");

app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
