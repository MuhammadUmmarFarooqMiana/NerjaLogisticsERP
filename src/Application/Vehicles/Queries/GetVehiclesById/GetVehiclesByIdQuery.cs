using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetVehiclesById;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant},{Roles.Supervisor}")]
public record GetVehiclesByIdQuery : IRequest<VehicleDto>
{
    public Guid Id { get; init; }
}
