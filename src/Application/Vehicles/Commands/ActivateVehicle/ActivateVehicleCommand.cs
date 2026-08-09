namespace NerjaLogisticsERP.Application.Vehicles.Commands.ActivateVehicle;

public record ActivateVehicleCommand : IRequest 
{ 
    public Guid Id { get; init; } 
}

