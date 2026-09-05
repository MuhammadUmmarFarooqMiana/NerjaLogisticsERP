namespace NerjaLogisticsERP.Application.Common.Interfaces;

public record AccessTokenResult(string Token, DateTimeOffset ExpiresAt);

public interface IJwtService
{
    AccessTokenResult GenerateAccessToken(Guid userId, string email, string fullName, IEnumerable<string> roles);
    string GenerateRefreshToken();
}
