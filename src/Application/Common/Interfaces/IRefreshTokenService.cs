namespace NerjaLogisticsERP.Application.Common.Interfaces;

public interface IRefreshTokenService
{
    Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken);
    Task<Guid?> ValidateAndRotateAsync(string rawToken, CancellationToken cancellationToken);
    Task RevokeAsync(string rawToken, CancellationToken cancellationToken);
}
