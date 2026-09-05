using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAllocation;

public class UpdateVehicleAllocationCommandHandler : IRequestHandler<UpdateVehicleAllocationCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateVehicleAllocationCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateVehicleAllocationCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _context.VehicleAllocationHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleAllocationHistory), request.Id.ToString());

        var newEmployee = await _context.Employees.FindAsync([request.EmployeeId], cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        // Only an active (not-yet-returned) allocation reflects who currently holds the
        // vehicle — editing a historical/returned record is just a data correction and
        // shouldn't touch anyone's current assignment.
        var isActive = allocation.ReturnedDate is null;
        var previousEmployeeId = allocation.EmployeeId;

        allocation.Update(request.EmployeeId, request.AssignedDate);

        if (isActive && previousEmployeeId != request.EmployeeId)
        {
            var previousEmployee = await _context.Employees.FindAsync([previousEmployeeId], cancellationToken);
            if (previousEmployee?.VehicleId == allocation.VehicleId)
                previousEmployee.UnassignVehicle();

            newEmployee.AssignVehicle(allocation.VehicleId);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
