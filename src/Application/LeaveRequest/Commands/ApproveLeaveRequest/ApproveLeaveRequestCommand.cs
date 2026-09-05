using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Domain.Constants;

namespace NerjaLogisticsERP.Application.LeaveRequest.Commands.ApproveLeaveRequest;

[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record ApproveLeaveRequestCommand : IRequest { public Guid LeaveRequestId { get; init; } }

