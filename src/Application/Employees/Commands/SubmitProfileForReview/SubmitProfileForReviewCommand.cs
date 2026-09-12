using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;

// No Roles on [Authorize] — any authenticated user with an Employee record may submit
// their own profile. UserId below is legacy wire shape only (kept so the existing
// {id}/submitProfile route and request body don't need to change) — the handler
// ignores it and always resolves the acting employee from _user.Id, never this field.
[Authorize]
public record SubmitProfileForReviewCommand : IRequest
{
    public Guid UserId { get; init; }
    public string IqamaNumber { get; init; } = string.Empty;
    public string PlatformIdNumber { get; init; } = string.Empty;
    public DateOnly? IdExpiryDate { get; init; }
    public DateOnly? IqamaExpiryDate { get; init; }
    public DateOnly? DrivingLicenseExpiryDate { get; init; }
    public DateOnly? InsuranceExpiryDate { get; init; }
}
