namespace NerjaLogisticsERP.Application.Auth.Commands.Login;

public record LoginResult(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken);

public record LoginCommand : IRequest<LoginResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    
}
