using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IApplicationDbContext _context;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IApplicationDbContext context)
    {
        _identityService = identityService;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
        _context = context;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var userId = await _identityService.ValidateCredentialsAsync(request.Email, request.Password);
        if (userId is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

        if (employee is null || employee.AccountStatus is AccountStatus.Terminated or AccountStatus.Suspended)
            throw new ForbiddenAccessException(); //May be Your account is pending approval or Suspended/Terminated

        var roles = await _identityService.GetRolesAsync(userId.Value);

        var accessToken = _jwtService.GenerateAccessToken(userId.Value, request.Email, employee.FullName, roles);
        var refreshToken = await _refreshTokenService.IssueAsync(userId.Value, cancellationToken);

        return new LoginResult(accessToken.Token, accessToken.ExpiresAt, refreshToken);
    }
}
