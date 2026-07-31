namespace NerjaLogisticsERP.Application.Auth.Commands.Refresh;

public record RefreshResult(string AccessToken, DateTimeOffset ExpiresAt, string RefreshToken);

public record RefreshCommand : IRequest<RefreshResult>
{
    public string RefreshToken { get; init; } = string.Empty;
}
