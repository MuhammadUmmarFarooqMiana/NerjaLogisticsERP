using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Platforms.Queries.GetPlatforms;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Platforms.Queries.GetPlatformById;

public class GetPlatformByIdQueryHandler : IRequestHandler<GetPlatformByIdQuery, PlatformDto>
{
    private readonly IApplicationDbContext _context;
    public GetPlatformByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PlatformDto> Handle(GetPlatformByIdQuery request, CancellationToken cancellationToken)
    {
        var platform = await _context.Platforms
            .Where(p => p.Id == request.Id)
            .Select(p => new PlatformDto { Id = p.Id, Name = p.Name })
            .FirstOrDefaultAsync(cancellationToken);

        return platform ?? throw new NotFoundException(nameof(Platform), request.Id.ToString());
    }
}
