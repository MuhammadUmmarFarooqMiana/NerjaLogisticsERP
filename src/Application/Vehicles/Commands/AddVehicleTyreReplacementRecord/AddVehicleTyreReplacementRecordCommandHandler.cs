using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleTyreReplacementRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public class AddVehicleTyreReplacementRecordCommandHandler : IRequestHandler<AddVehicleTyreReplacementRecordCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddVehicleTyreReplacementRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddVehicleTyreReplacementRecordCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Vehicle), request.VehicleId.ToString());

        var record = VehicleTyreReplacementHistory.Create(request.VehicleId, request.ReplacementDate, request.Odometer, request.NumberOfTyres, request.Cost);
        _context.VehicleTyreReplacementHistories.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
        return record.Id;
    }
}
