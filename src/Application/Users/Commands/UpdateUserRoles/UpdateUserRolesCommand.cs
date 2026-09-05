using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Users.Commands.UpdateUserRoles;

[Authorize(Roles = NerjaLogisticsERP.Domain.Constants.Roles.Administrator)]
public record UpdateUserRolesCommand : IRequest
{
    public Guid UserId { get; init; }
    public List<string> Roles { get; init; } = new();
}
