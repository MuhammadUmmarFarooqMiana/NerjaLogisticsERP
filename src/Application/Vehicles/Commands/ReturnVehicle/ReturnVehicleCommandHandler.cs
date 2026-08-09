using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public class ReturnVehicleCommandHandler : IRequestHandler<ReturnVehicleCommand>
{
    private readonly IApplicationDbContext _context;
    public ReturnVehicleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ReturnVehicleCommand request, CancellationToken cancellationToken)
    {
        var allocation = await _context.VehicleAllocationHistories.FindAsync(new object[] { request.AllocationId }, cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleAllocationHistory), request.AllocationId.ToString());

        allocation.ReturnVehicle(request.ReturnedDate);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
