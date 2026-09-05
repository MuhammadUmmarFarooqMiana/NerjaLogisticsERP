using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeactivateVehicle;

[Authorize(Roles = Roles.Administrator)]
public record DeactivateVehicleCommand : IRequest
{
    public Guid Id { get; init; }
}

