using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Queries;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant},{Roles.Supervisor}")]
public record GetVehicleHistoryQuery : IRequest<VehicleHistoryDto>
{
    public Guid VehicleId { get; init; }
}

