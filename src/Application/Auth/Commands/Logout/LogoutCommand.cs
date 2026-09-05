namespace NerjaLogisticsERP.Application.Auth.Commands.Logout;

public record LogoutCommand : IRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
