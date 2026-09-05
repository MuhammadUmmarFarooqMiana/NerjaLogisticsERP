using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.CreateVehicle;

[Authorize(Roles = Roles.Administrator)]
public record CreateVehicleCommand : IRequest<Guid>
{
    public string RegistrationNumber { get; init; } = string.Empty;
    public VehicleType VehicleType { get; init; }
}
