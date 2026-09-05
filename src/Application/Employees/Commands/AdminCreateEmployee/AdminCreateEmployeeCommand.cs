using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Employees.Commands.AdminCreateEmployee;

// Fast-tracks the account straight to Active — skips the Incomplete -> PendingReview rider
// self-registration pipeline, since an Administrator adding someone directly has already
// vetted them. Internally this still goes through Employee's normal SubmitProfileForReview +
// Approve lifecycle methods (just called back-to-back), so every domain invariant they enforce
// still applies — this isn't a separate, less-guarded code path.
[Authorize(Roles = NerjaLogisticsERP.Domain.Constants.Roles.Administrator)]
public record AdminCreateEmployeeCommand : IRequest<Guid>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public bool HasWhatsApp { get; init; } = true;
    public List<string> Roles { get; init; } = new();
    public string IqamaNumber { get; init; } = string.Empty;
    public Guid? PlatformId { get; init; }
    public string? PlatformIdNumber { get; init; }
    public DateOnly? JoiningDate { get; init; }
    public DateOnly? IdExpiryDate { get; init; }
    public DateOnly? IqamaExpiryDate { get; init; }
    public DateOnly? DrivingLicenseExpiryDate { get; init; }
    public DateOnly? InsuranceExpiryDate { get; init; }
}
