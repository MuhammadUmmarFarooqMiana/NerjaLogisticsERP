namespace NerjaLogisticsERP.Application.Employees.Queries;

public record EmployeeDetailDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = default!;
    public string AccountStatus { get; init; } = default!;
    public string PerformanceStatus { get; init; } = default!;
    public string? IqamaNumber { get; init; }
    public string? PlatformIdNumber { get; init; }
    public Guid? PlatformId { get; init; }
    public string? PlatformName { get; init; }
    public Guid? VehicleId { get; init; }
    public string? VehicleRegistrationNumber { get; init; }
    public Guid? SupervisorId { get; init; }
    public string? SupervisorName { get; init; }
    public DateOnly? JoiningDate { get; init; }
    public DateOnly? IdExpiryDate { get; init; }
    public DateOnly? IqamaExpiryDate { get; init; }
    public DateOnly? DrivingLicenseExpiryDate { get; init; }
    public DateOnly? InsuranceExpiryDate { get; init; }
    public DateTimeOffset? ProfileSubmittedAt { get; init; }
    public string RejectionReason { get; init; } = default!;

    // Sourced from ApplicationUser (Identity), not the Employee table itself —
    // populated via IIdentityService since Application can't reference the
    // Infrastructure-layer ApplicationUser type directly.
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool HasWhatsApp { get; init; }
    public bool EmailConfirmed { get; init; }
}
