namespace NerjaLogisticsERP.Domain.Entities;

public class VehicleServiceHistory : BaseAuditableEntity
{
    private VehicleServiceHistory() { }

    private VehicleServiceHistory(Guid vehicleId, DateOnly serviceDate, int odometer, string description, decimal cost)
    {
        VehicleId = vehicleId;
        ServiceDate = serviceDate;
        Odometer = odometer;
        Description = description;
        Cost = cost;
    }

    public Guid VehicleId { get; private set; }

    public Vehicle Vehicle { get; private set; } = default!;

    public DateOnly ServiceDate { get; private set; }

    public int Odometer { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public decimal Cost { get; private set; }

    public static VehicleServiceHistory Create(Guid vehicleId, DateOnly serviceDate, int odometer, string description, decimal cost)
    {
        if (vehicleId == Guid.Empty) throw new ArgumentException("VehicleId is required.", nameof(vehicleId));
        if (odometer < 0) throw new ArgumentException("Odometer cannot be negative.", nameof(odometer));
        if (cost < 0) throw new ArgumentException("Cost cannot be negative.", nameof(cost));

        return new VehicleServiceHistory(vehicleId, serviceDate, odometer, description, cost);
    }

    public void Update(DateOnly serviceDate, int odometer, string description, decimal cost)
    {
        if (odometer < 0) throw new ArgumentException("Odometer cannot be negative.", nameof(odometer));
        if (cost < 0) throw new ArgumentException("Cost cannot be negative.", nameof(cost));

        ServiceDate = serviceDate;
        Odometer = odometer;
        Description = description;
        Cost = cost;
    }
}
