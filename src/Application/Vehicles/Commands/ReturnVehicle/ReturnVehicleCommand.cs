namespace NerjaLogisticsERP.Application.Vehicles.Commands.ReturnVehicle;

public record ReturnVehicleCommand : IRequest 
{ 
    public Guid AllocationId { get; init; } 
    public DateOnly ReturnedDate { get; init; } 
}
