using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.DailyOrders.Commands.UpsertDailyOrder;

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
}
