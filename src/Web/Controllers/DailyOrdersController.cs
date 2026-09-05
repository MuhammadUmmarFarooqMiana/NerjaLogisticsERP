using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.DailyOrders.Commands.ApproveDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Commands.CloseMyDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Commands.CorrectDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Commands.RejectDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Queries;
using NerjaLogisticsERP.Application.DailyOrders.Queries.GetDailyOrderHistory;
using NerjaLogisticsERP.Application.DailyOrders.Queries.GetDailyOrders;
using NerjaLogisticsERP.Application.DailyOrders.Queries.GetMyDailyOrder;
using NerjaLogisticsERP.Application.DailyOrders.Queries.GetPendingApprovalsCount;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DailyOrdersController : ApiControllerBase
{
    [HttpPost("UpsertOrderCount")]
    public async Task<IActionResult> UpsertDailyOrder([FromBody] UpsertDailyOrderCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Daily order updated.", id });
    }

    [HttpGet("me")]
    public async Task<ActionResult<DailyOrderDto?>> GetMyDailyOrder()
        => Ok(await Mediator.Send(new GetMyDailyOrderQuery()));

    [HttpPost("close")]
    public async Task<IActionResult> CloseMyDailyOrder()
    {
        await Mediator.Send(new CloseMyDailyOrderCommand());
        return Ok(new { message = "Daily order closed." });
    }

    [HttpGet]
    public async Task<ActionResult<List<DailyOrderListItemDto>>> GetDailyOrders(
        [FromQuery] DateOnly? date, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetDailyOrdersQuery { Date = date, PageNumber = pageNumber, PageSize = pageSize });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("pending-approvals-count")]
    public async Task<ActionResult<int>> GetPendingApprovalsCount()
        => Ok(await Mediator.Send(new GetPendingDailyOrderApprovalsCountQuery()));

    [HttpGet("history")]
    public async Task<ActionResult<List<DailyOrderListItemDto>>> GetDailyOrderHistory(
        [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetDailyOrderHistoryQuery
        {
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id)
    {
        await Mediator.Send(new ApproveDailyOrderCommand { DailyOrderId = id });
        return Ok(new { message = "Daily order approved.", id });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] string reason)
    {
        await Mediator.Send(new RejectDailyOrderCommand { DailyOrderId = id, Reason = reason });
        return Ok(new { message = "Daily order rejected.", id });
    }

    [HttpPost("{id:guid}/correct")]
    public async Task<IActionResult> Correct(Guid id, [FromBody] CorrectDailyOrderCommand command)
    {
        await Mediator.Send(command with { DailyOrderId = id });
        return Ok(new { message = "Daily order corrected.", id });
    }
}
