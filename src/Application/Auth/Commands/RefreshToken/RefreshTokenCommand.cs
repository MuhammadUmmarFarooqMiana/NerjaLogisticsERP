namespace NerjaLogisticsERP.Application.Auth.Commands.RefreshToken;

public record RefreshTokenResult(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken);

public record RefreshTokenCommand : IRequest<RefreshTokenResult>
{
    public string RefreshToken { get; init; } = string.Empty;
}
