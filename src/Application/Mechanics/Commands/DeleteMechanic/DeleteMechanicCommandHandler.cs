using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Mechanics.Commands.DeleteMechanic;

public class DeleteMechanicCommandHandler : IRequestHandler<DeleteMechanicCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteMechanicCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteMechanicCommand request, CancellationToken cancellationToken)
    {
        var mechanic = await _context.Mechanics.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Mechanic), request.Id.ToString());

        var inUse = await _context.StockOuts.AnyAsync(s => s.MechanicId == request.Id, cancellationToken);
        if (inUse) throw new ConflictException("Cannot delete a mechanic that has stock-out records.");

        _context.Mechanics.Remove(mechanic);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
