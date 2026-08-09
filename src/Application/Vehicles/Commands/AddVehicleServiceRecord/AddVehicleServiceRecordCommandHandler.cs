using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleServiceRecord;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Accountant}")]
public class AddVehicleServiceRecordCommandHandler : IRequestHandler<AddVehicleServiceRecordCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public AddVehicleServiceRecordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(AddVehicleServiceRecordCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Vehicle), request.VehicleId.ToString());

        var record = VehicleServiceHistory.Create(request.VehicleId, request.ServiceDate, request.Odometer, request.Description, request.Cost);
        _context.VehicleServiceHistories.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
        return record.Id;
    }
}
