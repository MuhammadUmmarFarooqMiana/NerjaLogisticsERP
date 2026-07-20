namespace NerjaLogisticsERP.Domain.Entities;

public class VehicleAllocationHistory : BaseAuditableEntity
{
    private VehicleAllocationHistory()
    {
    }

    private VehicleAllocationHistory(
        Guid vehicleId,
        Guid employeeId,
        DateOnly assignedDate,DateOnly? returnedDate)
    {
        VehicleId = vehicleId;
        EmployeeId = employeeId;
        AssignedDate = assignedDate;
        ReturnedDate = returnedDate;
    }

    public Guid VehicleId { get; private set; }

    public Vehicle Vehicle { get; private set; } = default!;

    public Guid EmployeeId { get; private set; }

    public Employee Employee { get; private set; } = default!;

    public DateOnly AssignedDate { get; private set; }

    public DateOnly? ReturnedDate { get; private set; }

    public static VehicleAllocationHistory Create(
        Guid vehicleId,
        Guid employeeId,
        DateOnly assignedDate, DateOnly? returnedDate)
    {
        return new VehicleAllocationHistory(vehicleId, employeeId, assignedDate, returnedDate);
    }

    public void ReturnVehicle(DateOnly returnedDate)
    {
        ReturnedDate = returnedDate;
    }
}
