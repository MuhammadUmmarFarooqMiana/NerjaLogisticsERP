using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Infrastructure.Data;

namespace NerjaLogisticsERP.Infrastructure.Identity;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public RefreshTokenService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken)
    {
        var rawToken = GenerateRawToken();
        var expiryDays = _configuration.GetValue<int>("Jwt:RefreshTokenExpiryDays", 7);

        var entity = RefreshToken.Create(userId, Hash(rawToken), DateTimeOffset.UtcNow.AddDays(expiryDays));

        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return rawToken;
    }

    public async Task<Guid?> ValidateAndRotateAsync(string rawToken, CancellationToken cancellationToken)
    {
        var hash = Hash(rawToken);

        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (existing is null || !existing.IsActive)
            return null;

        existing.Revoke();
        await _context.SaveChangesAsync(cancellationToken);

        return existing.UserId;
    }

    public async Task RevokeAsync(string rawToken, CancellationToken cancellationToken)
    {
        var hash = Hash(rawToken);
        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash, cancellationToken);

        if (existing is not null && existing.IsActive)
        {
            existing.Revoke();
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static string GenerateRawToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string Hash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToBase64String(bytes);
    }
}
