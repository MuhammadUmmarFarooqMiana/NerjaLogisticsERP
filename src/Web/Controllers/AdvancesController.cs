using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Advances.Commands.CreateAdvance;
using NerjaLogisticsERP.Application.Advances.Queries.GetAdvances;
using NerjaLogisticsERP.Application.Advances.Queries.GetMyAdvances;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdvancesController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateAdvanceCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Advance recorded.", id });
    }

    [HttpGet]
    public async Task<ActionResult<List<AdvanceDto>>> GetAdvances(
        [FromQuery] Guid? employeeId, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetAdvancesQuery
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("me")]
    public async Task<ActionResult<List<AdvanceDto>>> GetMyAdvances(
        [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetMyAdvancesQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }
}
