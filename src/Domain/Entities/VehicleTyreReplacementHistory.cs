namespace NerjaLogisticsERP.Domain.Entities;

public class VehicleTyreReplacementHistory : BaseAuditableEntity
{
    private VehicleTyreReplacementHistory() { }
    private VehicleTyreReplacementHistory(Guid vehicleId, DateOnly replacementDate, int odometer, int numberOfTyres, decimal cost)
    {
        VehicleId = vehicleId;
        ReplacementDate = replacementDate;
        Odometer = odometer;
        NumberOfTyres = numberOfTyres;
        Cost = cost;
    }

    public Guid VehicleId { get; private set; }

    public Vehicle Vehicle { get; private set; } = default!;

    public DateOnly ReplacementDate { get; private set; }

    public int Odometer { get; private set; }

    public int NumberOfTyres { get; private set; }

    public decimal Cost { get; private set; }

    public static VehicleTyreReplacementHistory Create(Guid vehicleId, DateOnly replacementDate, int odometer, int numberOfTyres, decimal cost)
    {
        if (vehicleId == Guid.Empty) throw new ArgumentException("VehicleId is required.", nameof(vehicleId));
        if (odometer < 0) throw new ArgumentException("Odometer cannot be negative.", nameof(odometer));
        if(numberOfTyres <= 0) throw new ArgumentException("NumberOfTyres should be valid.",nameof(numberOfTyres));
        if (cost < 0) throw new ArgumentException("Cost cannot be negative.", nameof(cost));

        return new VehicleTyreReplacementHistory(vehicleId, replacementDate, odometer, numberOfTyres, cost);
    }
}
