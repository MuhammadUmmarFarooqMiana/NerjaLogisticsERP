namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.ApproveLeaveRequest;

public record ApproveLeaveRequestCommand : IRequest { public Guid LeaveRequestId { get; init; } }

