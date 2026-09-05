using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleAccidentRecord;

public class AddVehicleAccidentRecordCommandHandler : IRequestHandler<AddVehicleAccidentRecordCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddVehicleAccidentRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddVehicleAccidentRecordCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Vehicle), request.VehicleId.ToString());

        var record = VehicleAccidentHistory.Create(request.VehicleId, request.AccidentDate, request.Description, request.RepairCost);
        _context.VehicleAccidentHistories.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
        return record.Id;
    }
}
