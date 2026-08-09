namespace NerjaLogisticsERP.Domain.Entities;

public class VehicleAllocationHistory : BaseAuditableEntity
{
    private VehicleAllocationHistory()
    {
    }

    private VehicleAllocationHistory(
        Guid vehicleId,
        Guid employeeId,
        DateOnly assignedDate)
    {
        VehicleId = vehicleId;
        EmployeeId = employeeId;
        AssignedDate = assignedDate;
    }

    public Guid VehicleId { get; private set; }

    public Vehicle Vehicle { get; private set; } = default!;

    public Guid EmployeeId { get; private set; }

    public Employee Employee { get; private set; } = default!;

    public DateOnly AssignedDate { get; private set; }

    public DateOnly? ReturnedDate { get; private set; }

    public static VehicleAllocationHistory Create(Guid vehicleId, Guid employeeId, DateOnly assignedDate)
    {
        if (vehicleId == Guid.Empty) throw new ArgumentException("VehicleId is required.", nameof(vehicleId));
        if (employeeId == Guid.Empty) throw new ArgumentException("EmployeeId is required.", nameof(employeeId));

        return new VehicleAllocationHistory(vehicleId, employeeId, assignedDate);
    }

    public void ReturnVehicle(DateOnly returnedDate)
    {
        if (ReturnedDate is not null)
            throw new InvalidOperationException("This allocation has already been returned.");
        if (returnedDate < AssignedDate)
            throw new ArgumentException("ReturnedDate cannot be before AssignedDate.", nameof(returnedDate));

        ReturnedDate = returnedDate;
    }
}
