using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Mappings;
using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Vehicles.Queries;

namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetVehicles;

public class GetVehiclesQueryHandler : IRequestHandler<GetVehiclesQuery, PaginatedList<VehicleDto>>
{
    private readonly IApplicationDbContext _context;
    public GetVehiclesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedList<VehicleDto>> Handle(GetVehiclesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Vehicles.AsQueryable();
        if (request.ActiveOnly == true) query = query.Where(v => v.IsActive);

        return await query
            // Skip/Take needs a stable order to be meaningful across pages — there
            // wasn't one before pagination existed since the whole list always came back.
            .OrderBy(v => v.RegistrationNumber)
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
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
