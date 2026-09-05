using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Platforms.Commands.DeletePlatform;

[Authorize(Roles = Roles.Administrator)]
public record DeletePlatformCommand : IRequest
{
    public Guid Id { get; init; }
}
