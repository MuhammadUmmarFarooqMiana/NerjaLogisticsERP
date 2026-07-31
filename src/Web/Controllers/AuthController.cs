using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Auth.Commands.Login;
using NerjaLogisticsERP.Application.Auth.Commands.Refresh;
using NerjaLogisticsERP.Application.Employees.Commands.RegisterEmployee;

namespace NerjaLogisticsERP.Web.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(RegisterEmployeeCommand command)
    {
        var employeeId = await Mediator.Send(command);
        return CreatedAtAction(nameof(Register), new { id = employeeId }, employeeId);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshResult>> Refresh(RefreshCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
