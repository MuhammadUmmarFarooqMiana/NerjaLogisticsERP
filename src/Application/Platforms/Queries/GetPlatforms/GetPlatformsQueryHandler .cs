using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Application.Platforms.Queries.GetPlatforms;

public class GetPlatformsQueryHandler : IRequestHandler<GetPlatformsQuery, List<PlatformDto>>
{
    private readonly IApplicationDbContext _context;
    public GetPlatformsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<PlatformDto>> Handle(GetPlatformsQuery request, CancellationToken cancellationToken)
        => await _context.Platforms
            .OrderBy(p => p.Name)
            .Select(p => new PlatformDto { Id = p.Id, Name = p.Name })
            .ToListAsync(cancellationToken);
}
