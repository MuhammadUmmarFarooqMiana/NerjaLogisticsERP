using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Queries;

public class GetVehicleHistoryQueryHandler : IRequestHandler<GetVehicleHistoryQuery, VehicleHistoryDto>
{
    private readonly IApplicationDbContext _context;
    public GetVehicleHistoryQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<VehicleHistoryDto> Handle(GetVehicleHistoryQuery request, CancellationToken cancellationToken)
    {
        var exists = await _context.Vehicles.AnyAsync(v => v.Id == request.VehicleId, cancellationToken);
        if (!exists) throw new NotFoundException(nameof(Vehicle), request.VehicleId.ToString());

        return new VehicleHistoryDto
        {
            ServiceHistory = await _context.VehicleServiceHistories.Where(s => s.VehicleId == request.VehicleId)
                .Select(s => new ServiceRecordDto(s.Id, s.ServiceDate, s.Odometer, s.Description, s.Cost)).ToListAsync(cancellationToken),
            OilChanges = await _context.VehicleOilChangeHistories.Where(o => o.VehicleId == request.VehicleId)
                .Select(o => new OilChangeRecordDto(o.Id, o.ChangeDate, o.Odometer, o.Cost)).ToListAsync(cancellationToken),
            TyreReplacements = await _context.VehicleTyreReplacementHistories.Where(t => t.VehicleId == request.VehicleId)
                .Select(t => new TyreReplacementRecordDto(t.Id, t.ReplacementDate, t.Odometer, t.NumberOfTyres, t.Cost)).ToListAsync(cancellationToken),
            AccidentHistory = await _context.VehicleAccidentHistories.Where(a => a.VehicleId == request.VehicleId)
                .Select(a => new AccidentRecordDto(a.Id, a.AccidentDate, a.Description, a.RepairCost)).ToListAsync(cancellationToken)
        };
    }
}
