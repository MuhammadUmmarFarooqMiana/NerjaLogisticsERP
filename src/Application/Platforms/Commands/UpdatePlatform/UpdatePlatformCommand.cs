using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Platforms.Commands.UpdatePlatform;

[Authorize(Roles = Roles.Administrator)]
public record UpdatePlatformCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
