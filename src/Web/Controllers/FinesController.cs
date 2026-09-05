using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Fines.Commands.CreateFine;
using NerjaLogisticsERP.Application.Fines.Queries.GetFines;
using NerjaLogisticsERP.Application.Fines.Queries.GetMyFines;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FinesController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateFineCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Fine recorded.", id });
    }

    [HttpGet]
    public async Task<ActionResult<List<FineDto>>> GetFines(
        [FromQuery] Guid? employeeId, [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetFinesQuery
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
    public async Task<ActionResult<List<FineDto>>> GetMyFines(
        [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetMyFinesQuery
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
