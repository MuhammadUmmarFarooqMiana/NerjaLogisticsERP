using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Mechanics.Commands.CreateMechanic;

public class CreateMechanicCommandHandler : IRequestHandler<CreateMechanicCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateMechanicCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateMechanicCommand request, CancellationToken cancellationToken)
    {
        var mechanic = Mechanic.Create(request.Name, request.Phone, request.Email, request.Specialty, request.Address);
        _context.Mechanics.Add(mechanic);
        await _context.SaveChangesAsync(cancellationToken);
        return mechanic.Id;
    }
}
