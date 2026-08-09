using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Platforms.Commands.CreatePlatform;
using NerjaLogisticsERP.Application.Platforms.Commands.DeletePlatform;
using NerjaLogisticsERP.Application.Platforms.Commands.UpdatePlatform;
using NerjaLogisticsERP.Application.Platforms.Queries.GetPlatformById;
using NerjaLogisticsERP.Application.Platforms.Queries.GetPlatforms;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PlatformDto>>> GetAll()
        => Ok(await Mediator.Send(new GetPlatformsQuery()));

    [HttpGet("{id}")]
    public async Task<ActionResult<PlatformDto>> GetById(Guid id)
        => Ok(await Mediator.Send(new GetPlatformByIdQuery { Id = id }));

    [HttpPost]
    public async Task<IActionResult> Create(CreatePlatformCommand command)
    {
        var id = await Mediator.Send(command);
        return Ok(new { message = "Platform created.", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdatePlatformCommand command)
    {
        if (id != command.Id) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Platform updated.", id });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeletePlatformCommand { Id = id });
        return Ok(new { message = "Platform deleted.", id });
    }
}
