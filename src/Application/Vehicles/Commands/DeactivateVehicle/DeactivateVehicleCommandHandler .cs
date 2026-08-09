using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeactivateVehicle;

[Authorize(Roles = Roles.Administrator)]
public class DeactivateVehicleCommandHandler : IRequestHandler<DeactivateVehicleCommand>
{
    private readonly IApplicationDbContext _context;
    public DeactivateVehicleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeactivateVehicleCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _context.Vehicles.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), request.Id.ToString());
        vehicle.Deactivate();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
