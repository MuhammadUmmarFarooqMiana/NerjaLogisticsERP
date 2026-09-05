using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record ApproveEmployeeCommand : IRequest
{
    public Guid EmployeeId { get; init; }
    public DateOnly? JoiningDate { get; init; }

    // All optional — Employee is shared across Rider/Supervisor/Accountant and
    // not every role needs a platform/vehicle. When provided, Approve() runs
    // first (so AssignVehicle's Active-only guard is already satisfied), then
    // these assignments apply in the same command.
    // PlatformIdNumber is no longer entered here — the rider provides it
    // themselves during profile submission (SubmitProfileForReviewCommand).
    public Guid? PlatformId { get; init; }
    public Guid? VehicleId { get; init; }
    public Guid? SupervisorId { get; init; }
}
