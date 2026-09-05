using NerjaLogisticsERP.Application.Common.Models;
using NerjaLogisticsERP.Application.Common.Security;
using NerjaLogisticsERP.Application.LeaveRequest.Queries;
using NerjaLogisticsERP.Domain.Constants;
using NerjaLogisticsERP.Domain.Enums;

namespace NerjaLogisticsERP.Application.LeaveRequest.Queries.GetLeaveRequests;

// Review queue: Administrator sees every employee's requests, a plain
// Supervisor is scoped to their own reports (mirrors GetDailyOrdersQuery).
[Authorize(Roles = $"{Roles.Administrator},{Roles.Supervisor}")]
public record GetLeaveRequestsQuery : IRequest<PaginatedList<LeaveRequestDto>>
{
    public LeaveStatus? Status { get; init; }
    public Guid? EmployeeId { get; init; }
    /// <summary>Both null (the default) returns every row, matching pre-pagination behavior.</summary>
    public int? PageNumber { get; init; }
    public int? PageSize { get; init; }
}
