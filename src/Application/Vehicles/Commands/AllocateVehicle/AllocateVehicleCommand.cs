using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AllocateVehicle;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record AllocateVehicleCommand : IRequest<Guid>
{
    public Guid VehicleId { get; init; }
    public Guid EmployeeId { get; init; }
    public DateOnly AssignedDate { get; init; }
}
