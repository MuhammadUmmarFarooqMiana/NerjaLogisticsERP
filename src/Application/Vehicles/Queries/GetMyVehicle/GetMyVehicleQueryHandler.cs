using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Vehicles.Queries;

namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetMyVehicle;

public class GetMyVehicleQueryHandler : IRequestHandler<GetMyVehicleQuery, VehicleDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public GetMyVehicleQueryHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<VehicleDto?> Handle(GetMyVehicleQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.Id!.Value;

        return await _context.Employees
            .Where(e => e.UserId == userId && e.VehicleId != null)
            .Select(e => e.Vehicle!)
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
    }
}
