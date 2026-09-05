using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
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
