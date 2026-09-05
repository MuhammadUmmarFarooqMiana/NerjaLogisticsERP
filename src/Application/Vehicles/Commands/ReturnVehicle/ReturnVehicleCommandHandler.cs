using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;

public class ReturnVehicleCommandHandler : IRequestHandler<ReturnVehicleCommand>
{
    private readonly IApplicationDbContext _context;
    public ReturnVehicleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ReturnVehicleCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _context.VehicleAllocationHistories.FindAsync(new object[] { request.AllocationId }, cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleAllocationHistory), request.AllocationId.ToString());

        allocation.ReturnVehicle(request.ReturnedDate);

        var employee = await _context.Employees.FindAsync(new object[] { allocation.EmployeeId }, cancellationToken);
        if (employee?.VehicleId == allocation.VehicleId)
            employee.UnassignVehicle();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
