namespace NerjaLogisticsERP.Domain.Entities;

public class Employee : BaseAuditableEntity
{
    private Employee() { }

    private Employee(Guid userId, string fullName)
    {
        UserId = userId;
        FullName = fullName;
        RejectionReason = string.Empty;
    }

    public Guid UserId { get; private set; }

    public string FullName { get; private set; } = default!;

    public string? IqamaNumber { get; private set; }

    public string? PlatformIdNumber { get; private set; }

    public DateOnly? JoiningDate { get; private set; }

    public DateOnly? IdExpiryDate { get; private set; }

    public DateOnly? IqamaExpiryDate { get; private set; }

    public DateOnly? DrivingLicenseExpiryDate { get; private set; }

    public DateOnly? InsuranceExpiryDate { get; private set; }
    public DateTimeOffset? ProfileSubmittedAt { get; private set; }

    public string RejectionReason { get; private set; } = string.Empty;

    public Guid? PlatformId { get; private set; }
    public Platform? Platform { get; private set; }

    public Guid? VehicleId { get; private set; }
    public Vehicle? Vehicle { get; private set; }

    // Self-referencing: a Supervisor is also an Employee row.
    public Guid? SupervisorId { get; private set; }
    public Employee? Supervisor { get; private set; }

    public string? ProfilePictureStorageKey { get; private set; }

    public string? ProfilePictureContentType { get; private set; }

    public AccountStatus AccountStatus { get; private set; } = AccountStatus.Incomplete;

    // Meaningful for Riders; harmless default for Supervisor/Accountant.
    public PerformanceStatus PerformanceStatus { get; private set; } = PerformanceStatus.Yellow;

    public static Employee Create(Guid userId, string fullName)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Employee name is required.", nameof(fullName));

        var employee = new Employee(userId, fullName);
        employee.AddDomainEvent(new EmployeeCreatedEvent(employee));
        return employee;
    }

    /// <summary>
    /// Rider fills in the remaining profile details and submits for Admin/Supervisor review.
    /// Callable from Incomplete (first submission) or Rejected (resubmission after correction).
    /// </summary>
    public void SubmitProfileForReview(
        string iqamaNumber,
        string? platformIdNumber,
        DateOnly? idExpiryDate,
        DateOnly? iqamaExpiryDate,
        DateOnly? drivingLicenseExpiryDate,
        DateOnly? insuranceExpiryDate)
    {
        if (AccountStatus is not (AccountStatus.Incomplete or AccountStatus.Rejected))
            throw new InvalidOperationException($"Cannot submit a profile with status {AccountStatus}.");

        if (string.IsNullOrWhiteSpace(iqamaNumber))
            throw new ArgumentException("Iqama number is required.", nameof(iqamaNumber));

        IqamaNumber = iqamaNumber;
        // Riders must supply this at submission time (enforced by
        // SubmitProfileForReviewCommandValidator); AdminCreateEmployee only
        // sends it when a platform applies to the role being created, so this
        // stays a soft set here rather than another unconditional throw —
        // don't blank out an already-set value on resubmission if omitted.
        if (!string.IsNullOrWhiteSpace(platformIdNumber))
            PlatformIdNumber = platformIdNumber;
        IdExpiryDate = idExpiryDate;
        IqamaExpiryDate = iqamaExpiryDate;
        DrivingLicenseExpiryDate = drivingLicenseExpiryDate;
        InsuranceExpiryDate = insuranceExpiryDate;

        AccountStatus = AccountStatus.PendingReview;
        ProfileSubmittedAt = DateTimeOffset.UtcNow;
        RejectionReason = "";

        AddDomainEvent(new EmployeeProfileSubmittedEvent(this));
    }

    // PlatformIdNumber is no longer set here — it's collected from the rider
    // during SubmitProfileForReview. This just links the internal Platform
    // record, decoupled from the ID number so it can be assigned/changed at
    // approval time without touching what the rider already provided.
    public void AssignPlatform(Guid platformId)
    {
        if (platformId == Guid.Empty)
            throw new ArgumentException("PlatformId is required.", nameof(platformId));

        PlatformId = platformId;
    }

    public void AssignVehicle(Guid vehicleId)
    {
        if (AccountStatus != AccountStatus.Active)
            throw new InvalidOperationException("Cannot assign a vehicle to an employee who is not active.");

        if (vehicleId == Guid.Empty)
            throw new ArgumentException("VehicleId is required.", nameof(vehicleId));

        VehicleId = vehicleId;
    }

    public void UnassignVehicle()
    {
        VehicleId = null;
    }

    public void AssignSupervisor(Guid supervisorId)
    {
        if (supervisorId == Id)
            throw new InvalidOperationException("An employee cannot supervise themselves.");

        SupervisorId = supervisorId;
    }

    public void UpdateProfile(
    string iqamaNumber,
    Guid platformId,
    DateOnly? idExpiryDate,
    DateOnly? iqamaExpiryDate,
    DateOnly? drivingLicenseExpiryDate,
    DateOnly? insuranceExpiryDate)
    {
        if (ProfileSubmittedAt is not null)
            throw new InvalidOperationException("Cannot edit a profile that has already been submitted for review.");

        if (string.IsNullOrWhiteSpace(iqamaNumber))
            throw new ArgumentException("Iqama number is required.", nameof(iqamaNumber));
        if (platformId == Guid.Empty)
            throw new ArgumentException("Platform is required.", nameof(platformId));

        IqamaNumber = iqamaNumber;
        PlatformId = platformId;
        IdExpiryDate = idExpiryDate;
        IqamaExpiryDate = iqamaExpiryDate;
        DrivingLicenseExpiryDate = drivingLicenseExpiryDate;
        InsuranceExpiryDate = insuranceExpiryDate;
    }

    /// <summary>
    /// Administrator-driven correction, usable at any point in the lifecycle — unlike
    /// UpdateProfile (the rider's own pre-submission edit), this isn't blocked once the
    /// profile has been submitted/approved, since an admin may need to fix a typo or update
    /// details for an already-Active employee.
    /// </summary>
    public void AdminUpdateProfile(
        string fullName,
        string? iqamaNumber,
        Guid? platformId,
        DateOnly? idExpiryDate,
        DateOnly? iqamaExpiryDate,
        DateOnly? drivingLicenseExpiryDate,
        DateOnly? insuranceExpiryDate)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Employee name is required.", nameof(fullName));

        FullName = fullName;
        IqamaNumber = iqamaNumber;
        PlatformId = platformId;
        IdExpiryDate = idExpiryDate;
        IqamaExpiryDate = iqamaExpiryDate;
        DrivingLicenseExpiryDate = drivingLicenseExpiryDate;
        InsuranceExpiryDate = insuranceExpiryDate;
    }

    // Approve() now sets JoiningDate — defaults to today, but allows backdating
    // (e.g. Admin approving someone whose actual start date was last week)
    public void Approve(DateOnly? joiningDate = null)
    {

        if (AccountStatus != AccountStatus.PendingReview)
            throw new InvalidOperationException($"Cannot approve an employee with status {AccountStatus}.");

        if (ProfileSubmittedAt is null)
            throw new InvalidOperationException("Cannot approve a profile that has not been submitted for review.");

        AccountStatus = AccountStatus.Active;
        JoiningDate = joiningDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        AddDomainEvent(new EmployeeApprovedEvent(this));
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

    /// <summary>
    /// Points at the currently-stored file (if any) so the caller can delete it from storage
    /// before it's overwritten below — the entity itself only tracks the key, not the bytes.
    /// </summary>
    public string? SetProfilePicture(string storageKey, string contentType)
    {
        var previousStorageKey = ProfilePictureStorageKey;
        ProfilePictureStorageKey = storageKey;
        ProfilePictureContentType = contentType;
        return previousStorageKey;
    }

    /// <returns>The previously-stored key, so the caller can delete it from storage — null if there was none.</returns>
    public string? RemoveProfilePicture()
    {
        var previousStorageKey = ProfilePictureStorageKey;
        ProfilePictureStorageKey = null;
        ProfilePictureContentType = null;
        return previousStorageKey;
    }

    // --- AccountStatus states ---

    public void Reject(string reason)
    {
        if (AccountStatus != AccountStatus.PendingReview)
            throw new InvalidOperationException($"Cannot reject an employee with status {AccountStatus}.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("A rejection reason is required.", nameof(reason));

        AccountStatus = AccountStatus.Rejected;
        RejectionReason = reason;
        AddDomainEvent(new EmployeeRejectedEvent(this));
    }

    public void Suspend()
    {
        if (AccountStatus != AccountStatus.Active)
            throw new InvalidOperationException($"Cannot suspend an employee with status {AccountStatus}.");

        AccountStatus = AccountStatus.Suspended;
        AddDomainEvent(new EmployeeSuspendedEvent(this));
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
        AddDomainEvent(new EmployeeTerminatedEvent(this));
    }

}
