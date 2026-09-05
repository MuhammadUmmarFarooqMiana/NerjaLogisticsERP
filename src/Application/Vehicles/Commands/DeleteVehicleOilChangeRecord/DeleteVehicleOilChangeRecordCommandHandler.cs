using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleOilChangeRecord;

public class DeleteVehicleOilChangeRecordCommandHandler : IRequestHandler<DeleteVehicleOilChangeRecordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteVehicleOilChangeRecordCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(DeleteVehicleOilChangeRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleOilChangeHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleOilChangeHistory), request.Id.ToString());

        record.Delete(_user.Id);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
