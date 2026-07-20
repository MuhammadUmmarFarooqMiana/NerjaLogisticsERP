namespace NerjaLogisticsERP.Domain.Entities;

public class Employee : BaseAuditableEntity
{
    private Employee() { }

    private Employee(Guid userId, string fullName, string iqamaNumber, DateOnly joiningDate)
    {
        UserId = userId;
        FullName = fullName;
        IqamaNumber = iqamaNumber;
        JoiningDate = joiningDate;
    }

    public Guid UserId { get; private set; }

    public string FullName { get; private set; } = default!;
    public string IqamaNumber { get; private set; } = default!;
    public string? PlatformIdNumber { get; private set; }
    public DateOnly JoiningDate { get; private set; }

    public DateOnly? IdExpiryDate { get; private set; }
    public DateOnly? IqamaExpiryDate { get; private set; }
    public DateOnly? DrivingLicenseExpiryDate { get; private set; }
    public DateOnly? InsuranceExpiryDate { get; private set; }

    public Guid? PlatformId { get; private set; }
    public Platform? Platform { get; private set; }

    public Guid? VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    // Self-referencing: a Supervisor is also an Employee row.
    public Guid? SupervisorId { get; private set; }
    public Employee? Supervisor { get; private set; }

    public AccountStatus AccountStatus { get; private set; } = AccountStatus.PendingApproval;

    // Meaningful for Riders; harmless default for Supervisor/Accountant.
    public PerformanceStatus PerformanceStatus { get; private set; } = PerformanceStatus.Yellow;

    public static Employee Create(Guid userId, string fullName, string iqamaNumber, DateOnly joiningDate)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Employee name is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(iqamaNumber))
            throw new ArgumentException("Iqama number is required.", nameof(iqamaNumber));

        return new Employee(userId, fullName, iqamaNumber, joiningDate);
    }

    public void AssignPlatform(Guid platformId, string platformIdNumber)
    {
        if (platformId == Guid.Empty)
            throw new ArgumentException("PlatformId is required.", nameof(platformId));
        if (string.IsNullOrWhiteSpace(platformIdNumber))
            throw new ArgumentException("Platform ID number is required.", nameof(platformIdNumber));

        PlatformId = platformId;
        PlatformIdNumber = platformIdNumber;
    }

    public void AssignVehicle(Guid vehicleId)
    {
        if (vehicleId == Guid.Empty)
            throw new ArgumentException("VehicleId is required.", nameof(vehicleId));

        VehicleId = vehicleId;
    }

    public void AssignSupervisor(Guid supervisorId)
    {
        if (supervisorId == Id)
            throw new InvalidOperationException("An employee cannot supervise themselves.");

        SupervisorId = supervisorId;
    }

    public void UpdateExpiryDates(
        DateOnly? idExpiryDate,
        DateOnly? iqamaExpiryDate,
        DateOnly? drivingLicenseExpiryDate,
        DateOnly? insuranceExpiryDate)
    {
        IdExpiryDate = idExpiryDate;
        IqamaExpiryDate = iqamaExpiryDate;
        DrivingLicenseExpiryDate = drivingLicenseExpiryDate;
        InsuranceExpiryDate = insuranceExpiryDate;
    }

    public void UpdatePerformanceStatus(PerformanceStatus status)
    {
        PerformanceStatus = status;
    }

    // --- AccountStatus state machine ---

    public void Approve()
    {
        if (AccountStatus != AccountStatus.PendingApproval)
            throw new InvalidOperationException($"Cannot approve an employee with status {AccountStatus}.");

        AccountStatus = AccountStatus.Active;
    }

    public void Reject()
    {
        if (AccountStatus != AccountStatus.PendingApproval)
            throw new InvalidOperationException($"Cannot reject an employee with status {AccountStatus}.");

        AccountStatus = AccountStatus.Rejected;
    }

    public void Suspend()
    {
        if (AccountStatus != AccountStatus.Active)
            throw new InvalidOperationException($"Cannot suspend an employee with status {AccountStatus}.");

        AccountStatus = AccountStatus.Suspended;
    }

    public void Reactivate()
    {
        if (AccountStatus != AccountStatus.Suspended)
            throw new InvalidOperationException($"Cannot reactivate an employee with status {AccountStatus}.");

        AccountStatus = AccountStatus.Active;
    }

    public void Terminate()
    {
        if (AccountStatus is AccountStatus.Terminated or AccountStatus.Rejected)
            throw new InvalidOperationException($"Employee is already {AccountStatus}.");

        AccountStatus = AccountStatus.Terminated;
    }
}
