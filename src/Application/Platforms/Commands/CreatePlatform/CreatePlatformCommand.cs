using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Platforms.Commands.CreatePlatform;

[Authorize(Roles = Roles.Administrator)]
public record CreatePlatformCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
}
