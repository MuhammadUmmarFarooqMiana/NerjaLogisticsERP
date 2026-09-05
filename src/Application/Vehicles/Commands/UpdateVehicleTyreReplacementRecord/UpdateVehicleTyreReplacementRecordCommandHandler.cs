using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleTyreReplacementRecord;

public class UpdateVehicleTyreReplacementRecordCommandHandler : IRequestHandler<UpdateVehicleTyreReplacementRecordCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateVehicleTyreReplacementRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateVehicleTyreReplacementRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleTyreReplacementHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleTyreReplacementHistory), request.Id.ToString());

        record.Update(request.ReplacementDate, request.Odometer, request.NumberOfTyres, request.Cost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
