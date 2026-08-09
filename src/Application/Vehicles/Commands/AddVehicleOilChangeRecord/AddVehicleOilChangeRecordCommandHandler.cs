using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleOilChangeRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public class AddVehicleOilChangeRecordCommandHandler : IRequestHandler<AddVehicleOilChangeRecordCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddVehicleOilChangeRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddVehicleOilChangeRecordCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Vehicle), request.VehicleId.ToString());

        var record = VehicleOilChangeHistory.Create(request.VehicleId, request.ChangeDate, request.Odometer, request.Cost);
        _context.VehicleOilChangeHistories.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
        return record.Id;
    }
}
