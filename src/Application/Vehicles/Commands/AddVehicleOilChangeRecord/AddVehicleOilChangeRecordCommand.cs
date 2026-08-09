namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleOilChangeRecord;

public record AddVehicleOilChangeRecordCommand : IRequest<Guid>
{
    public Guid VehicleId { get; init; }
    public DateOnly ChangeDate { get; init; }
    public int Odometer { get; init; }
    public decimal Cost { get; init; }
}
