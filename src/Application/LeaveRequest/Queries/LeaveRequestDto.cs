namespace NerjaLogisticsERP.Application.LeaveRequest.Queries;

public record LeaveRequestDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = default!;
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public string? Reason { get; init; }
    public string Status { get; init; } = default!;
    public string? ReviewedByName { get; init; }
    public DateTimeOffset? ReviewedAt { get; init; }
    public string? RejectionReason { get; init; }
}
