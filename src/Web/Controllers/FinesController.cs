using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Fines.Commands.CreateFine;

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
}
