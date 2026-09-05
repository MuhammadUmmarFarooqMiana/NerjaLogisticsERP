using NerjaLogisticsERP.Application.Common.Exceptions;
using NerjaLogisticsERP.Application.Common.Interfaces;
using NerjaLogisticsERP.Domain.Entities;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.CreateVehicle;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateVehicleCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Vehicles.AnyAsync(v => v.RegistrationNumber == request.RegistrationNumber, cancellationToken);
        if (exists) throw new ConflictException($"A vehicle with registration '{request.RegistrationNumber}' already exists.");

        var vehicle = Vehicle.Create(request.RegistrationNumber, request.VehicleType);
        _context.Vehicles.Add(vehicle);
        await _context.SaveChangesAsync(cancellationToken);
        return vehicle.Id;
    }
}
