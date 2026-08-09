namespace NerjaLogisticsERP.Application.Vehicles.Queries.GetVehiclesById;


public record GetVehiclesByIdQuery : IRequest<List<VehicleDto>> 
{
    public Guid Id { get; init; }
    public bool? ActiveOnly { get; init; } 
}
