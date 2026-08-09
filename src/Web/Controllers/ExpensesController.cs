using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Expenses.Commands.CreateExpense;

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
}
