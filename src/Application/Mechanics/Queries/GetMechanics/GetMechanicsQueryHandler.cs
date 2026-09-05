using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;

namespace NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanics;

public class GetMechanicsQueryHandler : IRequestHandler<GetMechanicsQuery, PaginatedList<MechanicDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMechanicsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<MechanicDto>> Handle(GetMechanicsQuery request, CancellationToken cancellationToken)
        => await _context.Mechanics
            .OrderBy(m => m.Name)
            .Select(m => new MechanicDto(m.Id, m.Name, m.Phone, m.Email, m.Specialty, m.Address))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
}
