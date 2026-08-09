using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AllocateVehicle;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public class AllocateVehicleCommandHandler : IRequestHandler<AllocateVehicleCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AllocateVehicleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AllocateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _context.Vehicles.FindAsync(new object[] { request.VehicleId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), request.VehicleId.ToString());
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        var alreadyAllocated = await _context.VehicleAllocationHistories
            .AnyAsync(a => a.VehicleId == request.VehicleId && a.ReturnedDate == null, cancellationToken);
        if (alreadyAllocated)
            throw new ConflictException("This vehicle is already allocated to another employee. Return it first.");

        var allocation = VehicleAllocationHistory.Create(request.VehicleId, request.EmployeeId, request.AssignedDate);
        _context.VehicleAllocationHistories.Add(allocation);
        employee.AssignVehicle(request.VehicleId);

        await _context.SaveChangesAsync(cancellationToken);
        return allocation.Id;
    }
}
