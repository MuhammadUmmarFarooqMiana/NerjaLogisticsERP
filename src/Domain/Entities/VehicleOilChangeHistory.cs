namespace NerjaLogisticsERP.Domain.Entities;

public class VehicleOilChangeHistory : BaseAuditableEntity
{
    private VehicleOilChangeHistory() { }
    private VehicleOilChangeHistory(Guid vehicleId, DateOnly changeDate, int odometer, decimal cost)
    {
        VehicleId = vehicleId;
        ChangeDate = changeDate;
        Odometer = odometer;
        Cost = cost;
    }

    public Guid VehicleId { get; private set; }

    public Vehicle Vehicle { get; private set; } = default!;

    public DateOnly ChangeDate { get; private set; }

    public int Odometer { get; private set; }

    public decimal Cost { get; private set; }

    public static VehicleOilChangeHistory Create(Guid vehicleId, DateOnly changeDate, int odometer, decimal cost)
    {
        if (vehicleId == Guid.Empty) throw new ArgumentException("VehicleId is required.", nameof(vehicleId));
        if (odometer < 0) throw new ArgumentException("Odometer cannot be negative.", nameof(odometer));
        if (cost < 0) throw new ArgumentException("Cost cannot be negative.", nameof(cost));

        return new VehicleOilChangeHistory(vehicleId, changeDate, odometer, cost);
    }

    public void Update(DateOnly changeDate, int odometer, decimal cost)
    {
        if (odometer < 0) throw new ArgumentException("Odometer cannot be negative.", nameof(odometer));
        if (cost < 0) throw new ArgumentException("Cost cannot be negative.", nameof(cost));

        ChangeDate = changeDate;
        Odometer = odometer;
        Cost = cost;
    }
}
