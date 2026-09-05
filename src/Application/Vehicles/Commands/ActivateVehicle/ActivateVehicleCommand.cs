using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.ActivateVehicle;

[Authorize(Roles = Roles.Administrator)]
public record ActivateVehicleCommand : IRequest
{
    public Guid Id { get; init; }
}

