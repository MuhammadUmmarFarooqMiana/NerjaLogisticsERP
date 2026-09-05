using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleOilChangeRecord;

public class UpdateVehicleOilChangeRecordCommandHandler : IRequestHandler<UpdateVehicleOilChangeRecordCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateVehicleOilChangeRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateVehicleOilChangeRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleOilChangeHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleOilChangeHistory), request.Id.ToString());

        record.Update(request.ChangeDate, request.Odometer, request.Cost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
