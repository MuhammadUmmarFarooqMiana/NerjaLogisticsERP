using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Vehicles.Commands.DeactivateVehicle;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.ActivateVehicle;

public class ActivateVehicleCommandHandler : IRequestHandler<ActivateVehicleCommand>
{
    private readonly IApplicationDbContext _context;
    public ActivateVehicleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ActivateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _context.Vehicles.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), request.Id.ToString());
        vehicle.Activate();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
