namespace NerjaLogisticsERP.Domain.Entities;
using NerjaLogisticsERP.Domain.Common;
using NerjaLogisticsERP.Domain.Enums;

public class Vehicle : BaseAuditableEntity
{
    private Vehicle()
    {
    }

    private Vehicle(
        string registrationNumber,
        VehicleType vehicleType)
    {
        RegistrationNumber = registrationNumber;
        VehicleType = vehicleType;
    }

    public string RegistrationNumber { get; private set; } = string.Empty;

    public VehicleType VehicleType { get; private set; }

    public bool IsActive { get; private set; } = true;

    public static Vehicle Create(
        string registrationNumber,
        VehicleType vehicleType)
    {
        if (string.IsNullOrWhiteSpace(registrationNumber))
            throw new ArgumentException(nameof(registrationNumber));

        return new Vehicle(registrationNumber, vehicleType);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public ICollection<VehicleAllocationHistory> Allocations { get; private set; }
        = new List<VehicleAllocationHistory>();

    public ICollection<VehicleServiceHistory> ServiceHistory { get; private set; }
        = new List<VehicleServiceHistory>();

    public ICollection<VehicleOilChangeHistory> OilChanges { get; private set; }
        = new List<VehicleOilChangeHistory>();

    public ICollection<VehicleTyreReplacementHistory> TyreReplacements { get; private set; }
        = new List<VehicleTyreReplacementHistory>();

    public ICollection<VehicleAccidentHistory> AccidentHistory { get; private set; }
        = new List<VehicleAccidentHistory>();
}
