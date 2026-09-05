using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Expenses.Commands.CreateExpense;
using NerjaLogisticsERP.Application.Expenses.Queries.GetExpenses;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpensesController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Expense recorded.", id });
    }

    [HttpGet]
    public async Task<ActionResult<List<ExpenseDto>>> GetExpenses(
        [FromQuery] ExpenseCategory? category, [FromQuery] Guid? platformId,
        [FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetExpensesQuery
        {
            Category = category,
            PlatformId = platformId,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }
}
