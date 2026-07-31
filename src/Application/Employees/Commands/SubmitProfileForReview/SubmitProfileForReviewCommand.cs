namespace NerjaLogisticsERP.Application.Employees.Commands.SubmitProfileForReview;

public record SubmitProfileForReviewCommand : IRequest
{
    public Guid EmployeeId { get; init; }
    public string IqamaNumber { get; init; } = string.Empty;
    public DateOnly? IdExpiryDate { get; init; }
    public DateOnly? IqamaExpiryDate { get; init; }
    public DateOnly? DrivingLicenseExpiryDate { get; init; }
    public DateOnly? InsuranceExpiryDate { get; init; }
}
