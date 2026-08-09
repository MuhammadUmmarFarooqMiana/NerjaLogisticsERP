namespace NerjaLogisticsERP.Application.Vehicles.Commands.DeactivateVehicle;

public record DeactivateVehicleCommand : IRequest 
{ 
    public Guid Id { get; init; } 
}

