using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleServiceRecord;

public class UpdateVehicleServiceRecordCommandHandler : IRequestHandler<UpdateVehicleServiceRecordCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateVehicleServiceRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateVehicleServiceRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleServiceHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleServiceHistory), request.Id.ToString());

        record.Update(request.ServiceDate, request.Odometer, request.Description, request.Cost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
