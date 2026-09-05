using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleAccidentRecord;

public class DeleteVehicleAccidentRecordCommandHandler : IRequestHandler<DeleteVehicleAccidentRecordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteVehicleAccidentRecordCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(DeleteVehicleAccidentRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleAccidentHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleAccidentHistory), request.Id.ToString());

        record.Delete(_user.Id);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
