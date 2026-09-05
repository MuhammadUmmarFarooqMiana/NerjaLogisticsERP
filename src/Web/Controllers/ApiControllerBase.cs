using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator = null!;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    // Matches ASP.NET Core's default MVC JSON casing for the response body,
    // so consumers don't have to special-case this one header's property names.
    private static readonly JsonSerializerOptions PaginationHeaderJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Surfaces paging metadata via an X-Pagination response header, leaving the JSON body as
    /// the plain array every existing caller already expects — pair with `Ok(result.Items)`.
    /// </summary>
    protected void AddPaginationHeader<T>(PaginatedList<T> list)
    {
        var metadata = new
        {
            list.TotalCount,
            list.PageNumber,
            list.TotalPages,
            list.HasNextPage,
            list.HasPreviousPage,
        };

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata, PaginationHeaderJsonOptions));
    }
}
