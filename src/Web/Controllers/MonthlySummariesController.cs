using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.MonthlySummaries;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.GenerateMonthlySummary;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.MarkMonthlySummaryAsPaid;
using NerjaLogisticsERP.Application.MonthlySummaries.Commands.VerifyMonthlySummary;
using NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaries;
using NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMonthlySummaryById;
using NerjaLogisticsERP.Application.MonthlySummaries.Queries.GetMyMonthlySummaries;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MonthlySummariesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MonthlySummaryDto>>> GetAll(
        [FromQuery] Guid? employeeId, [FromQuery] int? year, [FromQuery] int? month, [FromQuery] MonthlySummaryStatus? status,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetMonthlySummariesQuery
        {
            EmployeeId = employeeId,
            Year = year,
            Month = month,
            Status = status,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MonthlySummaryDetailDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetMonthlySummaryByIdQuery { Id = id }));

    [HttpGet("me")]
    public async Task<ActionResult<List<MonthlySummaryDto>>> GetMine(
        [FromQuery] int? year, [FromQuery] int? month,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetMyMonthlySummariesQuery
        {
            Year = year,
            Month = month,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate(GenerateMonthlySummaryCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Monthly summary generated.", id });
    }

    [HttpPost("{id}/verify")]
    public async Task<IActionResult> Verify(Guid id)
    {
        await Mediator.Send(new VerifyMonthlySummaryCommand { Id = id });
        return Ok(new { message = "Monthly summary verified.", id });
    }

    [HttpPost("{id}/MarkAsPaid")]
    public async Task<IActionResult> MarkPaid(Guid id, [FromBody] string? paymentReference)
    {
        await Mediator.Send(new MarkMonthlySummaryAsPaidCommand { Id = id, PaymentReference = paymentReference });
        return Ok(new { message = "Monthly summary marked as paid.", id });
    }
}
