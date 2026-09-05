using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.UpdateVehicleAccidentRecord;

public class UpdateVehicleAccidentRecordCommandHandler : IRequestHandler<UpdateVehicleAccidentRecordCommand>
{
    private readonly IApplicationDbContext _context;
    public UpdateVehicleAccidentRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateVehicleAccidentRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleAccidentHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleAccidentHistory), request.Id.ToString());

        record.Update(request.AccidentDate, request.Description, request.RepairCost);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
