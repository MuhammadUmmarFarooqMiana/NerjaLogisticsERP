using NerjaLogisticsERP.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
    await app.SeedDevelopmentAdministratorAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

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

app.UseCors(static builder =>
    builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin()
        // AllowAnyHeader only covers request headers — response headers need explicit
        // exposure or browser JS can't read them cross-origin, even with AllowAnyOrigin.
        .WithExposedHeaders("X-Pagination"));

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
