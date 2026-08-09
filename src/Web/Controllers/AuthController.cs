using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Auth.Commands.Login;
using NerjaLogisticsERP.Application.Auth.Commands.RefreshToken;
using NerjaLogisticsERP.Application.Employees.Commands.RegisterEmployee;

namespace NerjaLogisticsERP.Web.Controllers;

public class AuthController : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(RegisterEmployeeCommand command)
    {
        var employeeId = await Mediator.Send(command);
        return Ok(new
        {
            Message = "Employee registered successfully.",
            employeeId = employeeId}
        );
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login(LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refreshtoken")]
    public async Task<ActionResult<RefreshTokenResult>> Refresh(RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
