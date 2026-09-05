using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Platforms.Queries.GetPlatforms;

[Authorize]
public record GetPlatformsQuery : IRequest<List<PlatformDto>>;
