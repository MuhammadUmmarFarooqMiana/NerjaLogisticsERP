using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Users.Commands.UpdateUserRoles;
using NerjaLogisticsERP.Application.Users.Queries;
using NerjaLogisticsERP.Application.Users.Queries.GetUsers;

namespace NerjaLogisticsERP.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UserListItemDto>>> GetUsers()
        => Ok(await Mediator.Send(new GetUsersQuery()));

    [HttpPut("{id:guid}/roles")]
    public async Task<IActionResult> UpdateRoles(Guid id, UpdateUserRolesCommand command)
    {
        if (id != command.UserId) return BadRequest();
        await Mediator.Send(command);
        return Ok(new { message = "Roles updated.", id });
    }
}
