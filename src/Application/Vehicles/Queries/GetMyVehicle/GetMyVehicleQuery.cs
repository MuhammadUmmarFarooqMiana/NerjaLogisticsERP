using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.Vehicles.Queries;

namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetMyVehicle;

// No id — always the caller's own assigned vehicle. Null result means no
// vehicle is currently assigned, not an error.
[Authorize]
public record GetMyVehicleQuery : IRequest<VehicleDto?>;
