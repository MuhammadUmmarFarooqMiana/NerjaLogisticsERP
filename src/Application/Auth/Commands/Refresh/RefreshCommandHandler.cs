using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Auth.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshResult>
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;
    private readonly IApplicationDbContext _context;

    public RefreshCommandHandler(
        IRefreshTokenService refreshTokenService,
        IIdentityService identityService,
        IJwtService jwtService,
        IApplicationDbContext context)
    {
        _refreshTokenService = refreshTokenService;
        _identityService = identityService;
        _jwtService = jwtService;
        _context = context;
    }

    public async Task<RefreshResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var userId = await _refreshTokenService.ValidateAndRotateAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // Re-check account status on every refresh, not just at login — if an
        // Administrator suspends someone mid-session, their next refresh should
        // fail rather than silently keep minting valid access tokens.
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

        if (employee is null || employee.AccountStatus != AccountStatus.Active)
            throw new ForbiddenAccessException();

        var email = await _identityService.GetUserNameAsync(userId.Value)
            ?? throw new UnauthorizedAccessException("User not found.");
        var roles = await _identityService.GetRolesAsync(userId.Value);

        var accessToken = _jwtService.GenerateAccessToken(userId.Value, email, roles);
        var newRefreshToken = await _refreshTokenService.IssueAsync(userId.Value, cancellationToken);

        return new RefreshResult(accessToken.Token, accessToken.ExpiresAt, newRefreshToken);
    }
}
