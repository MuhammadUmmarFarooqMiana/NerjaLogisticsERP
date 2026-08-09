namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.SubmitLeaveRequest;

public record SubmitLeaveRequestCommand : IRequest<Guid>
{
    public Guid EmployeeId { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public string? Reason { get; init; }
}
