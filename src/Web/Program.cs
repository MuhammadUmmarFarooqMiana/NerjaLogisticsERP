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
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

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

app.UseFileServer();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapDefaultEndpoints();
app.MapEndpoints(typeof(Program).Assembly);

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
