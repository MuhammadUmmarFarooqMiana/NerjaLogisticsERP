using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Advances.Commands.CreateAdvance;

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
}
