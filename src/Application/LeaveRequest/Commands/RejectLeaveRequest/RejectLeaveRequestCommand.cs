using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.RejectLeaveRequest;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record RejectLeaveRequestCommand : IRequest
{
    public Guid LeaveRequestId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
