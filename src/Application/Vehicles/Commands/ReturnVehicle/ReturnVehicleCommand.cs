using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record ReturnVehicleCommand : IRequest
{
    public Guid AllocationId { get; init; }
    public DateOnly ReturnedDate { get; init; }
}
