using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleTyreReplacementRecord;

public class DeleteVehicleTyreReplacementRecordCommandHandler : IRequestHandler<DeleteVehicleTyreReplacementRecordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteVehicleTyreReplacementRecordCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(DeleteVehicleTyreReplacementRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleTyreReplacementHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleTyreReplacementHistory), request.Id.ToString());

        record.Delete(_user.Id);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
