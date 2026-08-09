using NerjaLogisticsERP.Application.Platforms.Queries.GetPlatforms;

namespace NerjaLogisticsERP.Application.Platforms.Queries.GetPlatformById;

public record GetPlatformByIdQuery : IRequest<PlatformDto>
{
    public Guid Id { get; init; }
}
