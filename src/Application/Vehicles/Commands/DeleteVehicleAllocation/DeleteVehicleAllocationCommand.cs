using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleAllocation;

[Authorize(Roles = Roles.Administrator)]
public record DeleteVehicleAllocationCommand : IRequest
{
    public Guid Id { get; init; }
}
