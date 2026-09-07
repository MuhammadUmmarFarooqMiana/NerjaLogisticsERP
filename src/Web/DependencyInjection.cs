using System.Threading.RateLimiting;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Infrastructure.Data;
using NerjaLogisticsERP.Web.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

        // Safety net for exceptions ProblemDetailsExceptionHandler doesn't recognise (a genuine bug,
        // a DB outage, etc.): ExceptionHandlerMiddleware falls back to this service, which emits a
        // generic ProblemDetails body instead of leaking exception details or returning an empty
        // response. TraceId is attached here so it also covers every response ProblemDetailsExceptionHandler
        // writes via IProblemDetailsService — a support ticket that quotes it is enough to find the
        // matching UnhandledExceptionBehaviour log entry server-side, without exposing anything sensitive.
        builder.Services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context =>
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier);

        builder.Services.AddControllers();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi(options =>
        {
            options.AddOperationTransformer<ApiExceptionOperationTransformer>();
            options.AddOperationTransformer<IdentityApiOperationTransformer>();
        });

        builder.Services.AddCors();

        // "auth" policy: 5 requests per minute per client IP, applied to Login/Register.
        // A fixed window keyed on the IP itself (rather than one shared AddFixedWindowLimiter)
        // so one client hammering the endpoint can't exhaust the allowance for everyone else.
        // QueueLimit 0 rejects the 6th+ request outright (429) instead of queueing it — for a
        // login attempt, "try again in a bit" is correct; there's nothing worth waiting to process.
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("auth", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
