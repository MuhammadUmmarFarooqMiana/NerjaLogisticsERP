using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleAllocation;

public class DeleteVehicleAllocationCommandHandler : IRequestHandler<DeleteVehicleAllocationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteVehicleAllocationCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(DeleteVehicleAllocationCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _context.VehicleAllocationHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleAllocationHistory), request.Id.ToString());

        if (allocation.ReturnedDate is null)
        {
            var employee = await _context.Employees.FindAsync([allocation.EmployeeId], cancellationToken);
            if (employee?.VehicleId == allocation.VehicleId)
                employee.UnassignVehicle();
        }

        allocation.Delete(_user.Id);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
