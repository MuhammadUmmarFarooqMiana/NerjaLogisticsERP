using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanics;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Mechanics.Queries.GetMechanicById;

public class GetMechanicByIdQueryHandler : IRequestHandler<GetMechanicByIdQuery, MechanicDto>
{
    private readonly IApplicationDbContext _context;

    public GetMechanicByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<MechanicDto> Handle(GetMechanicByIdQuery request, CancellationToken cancellationToken)
    {
        var mechanic = await _context.Mechanics.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Mechanic), request.Id.ToString());

        return new MechanicDto(mechanic.Id, mechanic.Name, mechanic.Phone, mechanic.Email, mechanic.Specialty, mechanic.Address);
    }
}
