namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;

public record RejectLeaveRequestCommand : IRequest
{
    public Guid LeaveRequestId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
