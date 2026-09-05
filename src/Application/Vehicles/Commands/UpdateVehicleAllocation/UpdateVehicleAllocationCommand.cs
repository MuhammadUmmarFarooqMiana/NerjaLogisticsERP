using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAllocation;

// Reallocating a vehicle (changing who it's assigned to, or correcting a
// mistaken entry) is an Administrator-only action — unlike the initial
// Allocate/Return, which Supervisors can also perform.
[Authorize(Roles = Roles.Administrator)]
public record UpdateVehicleAllocationCommand : IRequest
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public DateOnly AssignedDate { get; init; }
}
