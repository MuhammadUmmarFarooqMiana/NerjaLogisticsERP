using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Employees.Commands.UpdateEmployee;

[Authorize]
public record UpdateEmployeeProfileCommand : IRequest
{
    public string IqamaNumber { get; init; } = string.Empty;
    public Guid PlatformId { get; init; }
    public string PlatformIdNumber { get; init; } = string.Empty;
    public DateOnly? IdExpiryDate { get; init; }
    public DateOnly? IqamaExpiryDate { get; init; }
    public DateOnly? DrivingLicenseExpiryDate { get; init; }
    public DateOnly? InsuranceExpiryDate { get; init; }
}
