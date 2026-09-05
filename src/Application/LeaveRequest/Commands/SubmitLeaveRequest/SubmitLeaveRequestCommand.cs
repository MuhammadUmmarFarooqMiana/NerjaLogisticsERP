using NerjaLogisticsERP.Application.Common.Security;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.SubmitLeaveRequest;

[Authorize]
public record SubmitLeaveRequestCommand : IRequest<Guid>
{
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public string? Reason { get; init; }
}
