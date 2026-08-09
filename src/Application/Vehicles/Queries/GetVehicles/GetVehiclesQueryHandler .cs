using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Vehicles.Queries;

namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetVehicles;

public class GetVehiclesQueryHandler : IRequestHandler<GetVehiclesQuery, List<VehicleDto>>
{
    private readonly IApplicationDbContext _context;
    public GetVehiclesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<VehicleDto>> Handle(GetVehiclesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Vehicles.AsQueryable();
        if (request.ActiveOnly == true) query = query.Where(v => v.IsActive);

        return await query
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
            .ToListAsync(cancellationToken);
    }
}
