namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetVehicles;

public record GetVehiclesQuery : IRequest<List<VehicleDto>> 
{ 
    public bool? ActiveOnly { get; init; } 
}
