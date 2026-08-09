namespace NerjaLogisticsERP.Application.Vehicles.Commands.AddVehicleTyreReplacementRecord;

public record AddVehicleTyreReplacementRecordCommand : IRequest<Guid>
{
    public Guid VehicleId { get; init; }
    public DateOnly ReplacementDate { get; init; }
    public int Odometer { get; init; }
    public int NumberOfTyres { get; init; }
    public decimal Cost { get; init; }
}
