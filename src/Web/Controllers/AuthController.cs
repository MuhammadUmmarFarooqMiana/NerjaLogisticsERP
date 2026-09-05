using Microsoft.AspNetCore.Mvc;
using NerjaLogisticsERP.Application.Auth.Commands.ChangePassword;
using NerjaLogisticsERP.Application.Auth.Commands.Login;
using NerjaLogisticsERP.Application.Auth.Commands.Logout;
using NerjaLogisticsERP.Application.Auth.Commands.RefreshToken;
using NerjaLogisticsERP.Application.Auth.Dtos;
using NerjaLogisticsERP.Application.Employees.Commands.RegisterEmployee;

namespace NerjaLogisticsERP.Web.Controllers;

public class AuthController : ApiControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<ActionResult<Guid>> Register(RegisterEmployeeCommand command)
    {
        var employeeId = await Mediator.Send(command);
        return Ok(new
        {
            Message = "Employee registered successfully.",
            employeeId = employeeId
        }
        );
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthTokenResponseDto>> Login(LoginCommand command)
    {
        var result = await Mediator.Send(command);
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(new AuthTokenResponseDto(result.AccessToken, result.ExpiresAt));
    }

    [HttpPost("refreshtoken")]
    public async Task<ActionResult<AuthTokenResponseDto>> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var result = await Mediator.Send(new RefreshTokenCommand { RefreshToken = refreshToken });
        SetRefreshTokenCookie(result.RefreshToken);
        return Ok(new AuthTokenResponseDto(result.AccessToken, result.ExpiresAt));
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
    {
        await Mediator.Send(command);
        return Ok(new { message = "Password changed successfully." });
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (!string.IsNullOrEmpty(refreshToken))
            await Mediator.Send(new LogoutCommand { RefreshToken = refreshToken });

        Response.Cookies.Delete(RefreshTokenCookieName, new CookieOptions { Path = "/api/auth" });
        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var expiryDays = HttpContext.RequestServices.GetRequiredService<IConfiguration>()
            .GetValue<int>("Jwt:RefreshTokenExpiryDays", 7);

        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/api/auth",
            Expires = DateTimeOffset.UtcNow.AddDays(expiryDays)
        });
    }
}
