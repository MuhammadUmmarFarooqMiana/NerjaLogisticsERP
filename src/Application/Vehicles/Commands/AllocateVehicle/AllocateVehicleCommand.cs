namespace NerjaLogisticsERP.Application.Vehicles.Commands.AllocateVehicle;

public record AllocateVehicleCommand : IRequest<Guid>
{
    public Guid VehicleId { get; init; }
    public Guid EmployeeId { get; init; }
    public DateOnly AssignedDate { get; init; }
}
