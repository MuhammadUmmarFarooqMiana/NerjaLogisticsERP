using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeleteVehicleServiceRecord;

public class DeleteVehicleServiceRecordCommandHandler : IRequestHandler<DeleteVehicleServiceRecordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;

    public DeleteVehicleServiceRecordCommandHandler(IApplicationDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task Handle(DeleteVehicleServiceRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _context.VehicleServiceHistories.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException(nameof(VehicleServiceHistory), request.Id.ToString());

        record.Delete(_user.Id);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
