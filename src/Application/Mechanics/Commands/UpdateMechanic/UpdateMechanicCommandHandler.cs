using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Mechanics.Commands.UpdateMechanic;

public class UpdateMechanicCommandHandler : IRequestHandler<UpdateMechanicCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateMechanicCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(UpdateMechanicCommand request, CancellationToken cancellationToken)
    {
        var mechanic = await _context.Mechanics.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Mechanic), request.Id.ToString());

        mechanic.Update(request.Name, request.Phone, request.Email, request.Specialty, request.Address);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
