using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Platforms.Queries.GetPlatforms;

namespace NerjaLogisticsERP.Application.Platforms.Queries.GetPlatformById;

[Authorize]
public record GetPlatformByIdQuery : IRequest<PlatformDto>
{
    public Guid Id { get; init; }
}
