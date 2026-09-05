using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetVehiclesById;

public class GetVehiclesByIdQueryHandler : IRequestHandler<GetVehiclesByIdQuery, VehicleDto>
{
    private readonly IApplicationDbContext _context;
    public GetVehiclesByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<VehicleDto> Handle(GetVehiclesByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _context.Vehicles
            .Where(v => v.Id == request.Id)
            .Select(v => new VehicleDto
            {
                Id = v.Id,
                RegistrationNumber = v.RegistrationNumber,
                VehicleType = v.VehicleType.ToString(),
                IsActive = v.IsActive,
                AssignedEmployeeName = _context.Employees
                    .Where(e => e.VehicleId == v.Id)
                    .Select(e => e.FullName)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return vehicle ?? throw new NotFoundException(nameof(Vehicle), request.Id.ToString());
    }
}
