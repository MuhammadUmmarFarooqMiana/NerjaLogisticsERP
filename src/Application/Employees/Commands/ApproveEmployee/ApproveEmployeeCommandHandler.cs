using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Employees.Commands.ApproveEmployee;

public class ApproveEmployeeCommandHandler : IRequestHandler<ApproveEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public ApproveEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ApproveEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees.FindAsync(new object[] { request.EmployeeId }, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), request.EmployeeId.ToString());

        if (request.PlatformId.HasValue &&
            !await _context.Platforms.AnyAsync(p => p.Id == request.PlatformId, cancellationToken))
            throw new NotFoundException(nameof(Platform), request.PlatformId.Value.ToString());

        if (request.VehicleId.HasValue &&
            !await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken))
            throw new NotFoundException(nameof(Vehicle), request.VehicleId.Value.ToString());

        if (request.SupervisorId.HasValue &&
            !await _context.Employees.AnyAsync(e => e.Id == request.SupervisorId, cancellationToken))
            throw new NotFoundException(nameof(Employee), request.SupervisorId.Value.ToString());

        // Approve first — AssignVehicle requires an Active account, so the
        // status transition must happen before any assignment below.
        employee.Approve(request.JoiningDate);

        if (request.PlatformId.HasValue)
            employee.AssignPlatform(request.PlatformId.Value);

        if (request.VehicleId.HasValue)
            employee.AssignVehicle(request.VehicleId.Value);

        if (request.SupervisorId.HasValue)
            employee.AssignSupervisor(request.SupervisorId.Value);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
