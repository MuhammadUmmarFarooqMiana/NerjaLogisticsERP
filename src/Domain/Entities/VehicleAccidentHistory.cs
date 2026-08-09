namespace NerjaLogisticsERP.Domain.Entities;

public class VehicleAccidentHistory : BaseAuditableEntity
{
    private VehicleAccidentHistory(Guid vehicleId, DateOnly accidentDate, string description, decimal repairCost)
    {
        VehicleId = vehicleId;
        AccidentDate =  accidentDate;
        Description = description;
        RepairCost = repairCost;
    }

    private VehicleAccidentHistory() { }

    public Guid VehicleId { get; private set; }

    public Vehicle Vehicle { get; private set; } = default!;

    public DateOnly AccidentDate { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public decimal RepairCost { get; private set; }

    public static VehicleAccidentHistory Create(Guid vehicleId, DateOnly accidentDate, string description, decimal repairCost)
    {
        if (vehicleId == Guid.Empty) throw new ArgumentException("VehicleId is required.", nameof(vehicleId));
        if (repairCost < 0) throw new ArgumentException("Cost cannot be negative.", nameof(repairCost));

        return new VehicleAccidentHistory(vehicleId, accidentDate, description, repairCost);
    }

}
