namespace NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;

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
