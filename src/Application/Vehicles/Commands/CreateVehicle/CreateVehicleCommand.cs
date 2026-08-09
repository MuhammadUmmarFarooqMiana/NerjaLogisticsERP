using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.Vehicles.Commands.CreateVehicle;

public record CreateVehicleCommand : IRequest<Guid>
{
    public string RegistrationNumber { get; init; } = string.Empty;
    public VehicleType VehicleType { get; init; }
}
