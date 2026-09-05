using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Mechanics.Commands.CreateMechanic;
using NerjaLogisticsERP.Application.Mechanics.Commands.DeleteMechanic;
using NerjaLogisticsERP.Application.Mechanics.Commands.UpdateMechanic;
using NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanicById;
using NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanics;

namespace NerjaLogisticsERP.Web.Controllers;

public class MechanicsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MechanicDto>>> GetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        var result = await Mediator.Send(new GetMechanicsQuery { PageNumber = pageNumber, PageSize = pageSize });
        AddPaginationHeader(result);
        return Ok(result.Items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MechanicDto>> GetById(Guid id) => Ok(await Mediator.Send(new GetMechanicByIdQuery { Id = id }));

    [HttpPost]
    public async Task<IActionResult> Create(CreateMechanicCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Mechanic created.", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateMechanicCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Mechanic updated.", id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteMechanicCommand { Id = id });
        return Ok(new { message = "Mechanic deleted.", id });
    }
}
