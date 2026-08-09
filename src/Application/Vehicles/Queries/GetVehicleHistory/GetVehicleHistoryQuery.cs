namespace NerjaLogisticsERP.Application.Vehicles.Queries;

public record GetVehicleHistoryQuery : IRequest<VehicleHistoryDto> 
{ 
    public Guid VehicleId { get; init; } 
}

