using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Auth.Commands.ChangePassword;

// No Roles on [Authorize] — any authenticated user may change their own password.
// The handler scopes to _user.Id, so there's no way to target anyone else's account.
[Authorize]
public record ChangePasswordCommand : IRequest
{
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
    public string ConfirmNewPassword { get; init; } = string.Empty;
}
