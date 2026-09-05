namespace NerjaLogisticsERP.Application.Employees.Queries;

public record EmployeeListItemDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = default!;
    public string AccountStatus { get; init; } = default!;
    public string? IqamaNumber { get; init; }
    public string? PlatformName { get; init; }
    public DateOnly? JoiningDate { get; init; }
    /// <summary>
    /// Identity roles (Domain.Constants.Roles member names) held by this employee's
    /// user account — e.g. used by the Dashboard to break down "Active Riders"
    /// and per-platform headcount by role, since Employee itself has no role field
    /// (roles live on ApplicationUser/Identity).
    /// </summary>
    public List<string> Roles { get; init; } = new();
}
