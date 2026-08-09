using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Inventory.Commands.CreateInventoryItem;
using NerjaLogisticsERP.Application.Inventory.Commands.RecordStockIn;
using NerjaLogisticsERP.Application.Inventory.Commands.RecordStockOut;
using NerjaLogisticsERP.Application.Inventory.Queries.GetInventoryItems;
using NerjaLogisticsERP.Application.Inventory.Queries.GetStockHistory;
using NerjaLogisticsERP.Application.Inventory.Queries.GetStockLedger;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ApiControllerBase
{
    [HttpGet("items")]
    public async Task<ActionResult<List<InventoryItemDto>>> GetItems([FromQuery] bool? lowStockOnly)
        => Ok(await Mediator.Send(new GetInventoryItemsQuery { LowStockOnly = lowStockOnly }));

    [HttpPost("items")]
    public async Task<IActionResult> CreateItem(CreateInventoryItemCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Inventory item created.", id });
    }

    [HttpGet("items/{itemId}/history")]
    public async Task<ActionResult<List<StockMovementDto>>> GetHistory(Guid itemId)
        => Ok(await Mediator.Send(new GetStockHistoryQuery { ItemId = itemId }));

    [HttpPost("stock-in")]
    public async Task<IActionResult> StockIn(RecordStockInCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Stock received.", id });
    }

    [HttpPost("stock-out")]
    public async Task<IActionResult> StockOut(RecordStockOutCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Stock issued.", id });
    }

    [HttpGet("ledger")]
    public async Task<ActionResult<List<StockLedgerEntryDto>>> GetLedger(
    [FromQuery] Guid? itemId, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
    => Ok(await Mediator.Send(new GetStockLedgerQuery { ItemId = itemId, FromDate = fromDate, ToDate = toDate }));
}
